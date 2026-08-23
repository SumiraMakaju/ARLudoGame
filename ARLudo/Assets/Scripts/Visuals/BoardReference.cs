using System.Collections.Generic;
using UnityEngine;
using ARLudo.Core;

namespace ARLudo.Visuals
{
    public class BoardReference : MonoBehaviour
    {
        public Transform[] redYardSlots = new Transform[4];
        public Transform[] greenYardSlots = new Transform[4];
        public Transform[] yellowYardSlots = new Transform[4];
        public Transform[] blueYardSlots = new Transform[4];
        public float pawnYOffset = 0.015f;

        private Dictionary<int, Transform> tileLookup = new();

        public void BuildLookup()
        {
            tileLookup.Clear();
            foreach (var marker in GetComponentsInChildren<TileMarker>())
                tileLookup[marker.tileIndex] = marker.transform;
        }

        public Vector3 GetTilePosition(int tileIndex)
        {
            if (tileLookup.TryGetValue(tileIndex, out Transform t))
                return t.position + Vector3.up * pawnYOffset;
            return transform.position;
        }

        public Vector3 GetYardPosition(PlayerColor color, int pawnLocalIndex)
        {
            Transform[] slots = color switch
            {
                PlayerColor.Red => redYardSlots,
                PlayerColor.Green => greenYardSlots,
                PlayerColor.Yellow => yellowYardSlots,
                PlayerColor.Blue => blueYardSlots,
                _ => null
            };
            if (slots != null && pawnLocalIndex < slots.Length && slots[pawnLocalIndex] != null)
                return slots[pawnLocalIndex].position + Vector3.up * pawnYOffset;
            return transform.position;
        }

        public static Color GetPlayerColor(PlayerColor color)
        {
            return color switch
            {
                PlayerColor.Red => new Color(0.9f, 0.15f, 0.15f),
                PlayerColor.Green => new Color(0.1f, 0.75f, 0.2f),
                PlayerColor.Yellow => new Color(0.95f, 0.85f, 0.1f),
                PlayerColor.Blue => new Color(0.15f, 0.4f, 0.9f),
                _ => Color.white
            };
        }
    }
}