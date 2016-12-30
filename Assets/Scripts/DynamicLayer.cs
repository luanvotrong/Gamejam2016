using System;
using System.Collections.Generic;
using System.Linq;
using MathUtil;
using UnityEngine;

namespace Assets.Scripts
{
    // Contains movable pixels of animated effects, drawn over terrain layer
    public class DynamicLayer
    {
        public DynamicEntryMap Data { get; private set; }
        public List<Vector2I> ChangeList { get; private set; }

        public DynamicLayer()
        {
            this.Data = new DynamicEntryMap();
            this.ChangeList = new List<Vector2I>();
        }

        public Color32 GetAt(Vector2I coord)
        {
            return this.Data.GetColorAt(coord);
        }

        /*
        public Color32? GetTopAt(Vector2I coord)
        {
            List<DynamicLayerEntry> entries;
            if (this.Data.TryGetValue(coord, out entries))
            {
                var entry = entries.Or();
                if (entry != null)
                    return entry.Color;
            }
            return null;
        }*/

        public void MoveTo(DynamicLayerEntry entry, Vector3I oldPosition)
        {
            Profiler.BeginSample("DynamicLayer.MoveTo");
            this.ChangeList.Add(entry.Position.ToVector2I());
            if (!oldPosition.Equals(entry.Position))
            {
                this.ChangeList.Add(oldPosition.ToVector2I());
                this.Data.UpdatePosition(entry, oldPosition);
            }
            Profiler.EndSample();
        }

        public DynamicLayerEntry CreateDynamicEntry(Vector3I coord, Color32 color)
        {
            Profiler.BeginSample("DynamicLayer.CreateDynamicEntry");
            var entry = new DynamicLayerEntry(coord, color, this);
            this.Data.Add(entry);
            Profiler.EndSample();
            return entry;
        }

        public void RemoveDynamicEntry(DynamicLayerEntry entry)
        {
            Profiler.BeginSample("DynamicLayer.RemoveDynamicEntry");
            entry.Delete();
            this.Data.Remove(entry);
            Profiler.EndSample();
        }
    }

    public class DynamicEntryMap
    {
        private Dictionary<Vector2I, List<DynamicLayerEntry>> map = new Dictionary<Vector2I, List<DynamicLayerEntry>>();

        public Color32 GetColorAt(Vector2I coord)
        {
            List<DynamicLayerEntry> entries;
            if (this.map.TryGetValue(coord, out entries))
            {
                if (entries.Count > 0)
                    return entries[0].Color;
            }
            return default(Color32);
        }

        public DynamicLayerEntry GetAt(Vector2I coord)
        {
            List<DynamicLayerEntry> entries;
            if (this.map.TryGetValue(coord, out entries))
            {
                return entries.FirstOrDefault();
            }
            return null;
        }

        public void Add(DynamicLayerEntry entry)
        {
            Profiler.BeginSample("DynamicEntryMap.Add");
            List<DynamicLayerEntry> entries;
            Vector2I pos = entry.Position.ToVector2I();
            if (!this.map.TryGetValue(pos, out entries))
            {
                entries = new List<DynamicLayerEntry>();
                this.map[pos] = entries;
            }
            entries.Add(entry);
            Profiler.EndSample();
        }

        public void UpdatePosition(DynamicLayerEntry entry, Vector3I oldPosition)
        {
            if (oldPosition.ToVector2I().Equals(entry.Position.ToVector2I()))
                return;

            this.EraseAt(entry, oldPosition.ToVector2I());
            // Now reinsert with updated position
            this.Add(entry);

        }

        public void EraseAt(DynamicLayerEntry entry, Vector2I position)
        {
            Profiler.BeginSample("DynamicEntryMap.EraseAt");
   
            List<DynamicLayerEntry> entries;
            // Find the entry in our map
            if (this.map.TryGetValue(position, out entries))
            {
                int index = entries.IndexOf(entry);
                if (index == -1)
                    throw new Exception("UpdatePosition did not find original entry");

                // First erase original entry
                if (entries.Count == 1)
                    entries.Clear();
                else
                {   // Swap-erase
                    entries[index] = entries.Last();
                    entries.RemoveAt(entries.Count - 1);
                }
                Profiler.EndSample();
                return;
            }
            throw new Exception("UpdatePosition did not find original entry");
        }

        public void Remove(DynamicLayerEntry entry)
        {
            EraseAt(entry, entry.Position.ToVector2I());
        }
    }

    // One movable pixel of the terrain layer
    public class DynamicLayerEntry
    {
        public Vector3I Position { get; private set; }
        public Color32 Color { get; private set; }
        public DynamicLayer ParentLayer { get; private set; }

        public DynamicLayerEntry(Vector3I position, Color32 color, DynamicLayer parent)
        {
            this.Color = color;
            this.Position = position;
            this.ParentLayer = parent;
            InvalidateCurrentPosition();
        }

        public void Delete()
        {
            InvalidateCurrentPosition();
            this.ParentLayer = null;
        }
        /*
        public int CompareTo(DynamicLayerEntry other)
        {
            return this.Position.z.CompareTo(other.Position.z);
        }
        */

        public void MoveTo(Vector2I newPosition)
        {
            Vector3I oldPosition = this.Position;
            this.Position = new Vector3I(newPosition.x, newPosition.y, this.Position.z);
            InvalidateAndMove(oldPosition);
        }
        public void MoveTo(Vector3I newPosition)
        {
            Vector3I oldPosition = this.Position;
            this.Position = newPosition;
            InvalidateAndMove(oldPosition);
        }

        public void ChangeColor(Color32 newColor)
        {
            this.Color = newColor;
            InvalidateCurrentPosition();
        }

        private void InvalidateCurrentPosition()
        {
            this.ParentLayer.MoveTo(this, this.Position);
        }

        private void InvalidateAndMove(Vector3I oldPosition)
        {
            this.ParentLayer.MoveTo(this, oldPosition);
        }
    }

}