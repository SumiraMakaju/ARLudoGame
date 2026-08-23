using UnityEngine;
using UnityEditor;
using ARLudo.Core;
using ARLudo.Visuals;

namespace ARLudo.Editor
{
    public class BoardSetupTool
    {
        private static readonly Vector3[] Perimeter = {
            new(-1,0,-7), new(-1,0,-6), new(-1,0,-5), new(-1,0,-4),
            new(-1,0,-3), new(-1,0,-2), new(-2,0,-1), new(-3,0,-1),
            new(-4,0,-1), new(-5,0,-1), new(-6,0,-1), new(-7,0,-1),
            new(-7,0,0),  new(-7,0,1),  new(-6,0,1),  new(-5,0,1),
            new(-4,0,1),  new(-3,0,1),  new(-2,0,1),  new(-1,0,2),
            new(-1,0,3),  new(-1,0,4),  new(-1,0,5),  new(-1,0,6),
            new(-1,0,7),  new(0,0,7),   new(1,0,7),   new(1,0,6),
            new(1,0,5),   new(1,0,4),   new(1,0,3),   new(1,0,2),
            new(2,0,1),   new(3,0,1),   new(4,0,1),   new(5,0,1),
            new(6,0,1),   new(7,0,1),   new(7,0,0),   new(7,0,-1),
            new(6,0,-1),  new(5,0,-1),  new(4,0,-1),  new(3,0,-1),
            new(2,0,-1),  new(1,0,-2),  new(1,0,-3),  new(1,0,-4),
            new(1,0,-5),  new(1,0,-6),  new(1,0,-7),  new(0,0,-7)
        };
        private static readonly Vector3[][] HomeCor = {
            new[]{ new Vector3(0,0,-6), new(0,0,-5), new(0,0,-4), new(0,0,-3), new(0,0,-2), new(0,0,-1) },
            new[]{ new Vector3(-6,0,0), new(-5,0,0), new(-4,0,0), new(-3,0,0), new(-2,0,0), new(-1,0,0) },
            new[]{ new Vector3(0,0,6),  new(0,0,5),  new(0,0,4),  new(0,0,3),  new(0,0,2),  new(0,0,1) },
            new[]{ new Vector3(6,0,0),  new(5,0,0),  new(4,0,0),  new(3,0,0),  new(2,0,0),  new(1,0,0) }
        };
        private static readonly Vector3[][] Yards = {
            new[]{ new Vector3(-5,0,-5), new(-3.5f,0,-5), new(-5,0,-3.5f), new(-3.5f,0,-3.5f) },
            new[]{ new Vector3(-5,0,3.5f), new(-3.5f,0,3.5f), new(-5,0,5), new(-3.5f,0,5) },
            new[]{ new Vector3(3.5f,0,3.5f), new(5,0,3.5f), new(3.5f,0,5), new(5,0,5) },
            new[]{ new Vector3(3.5f,0,-5), new(5,0,-5), new(3.5f,0,-3.5f), new(5,0,-3.5f) }
        };
        private static readonly int[] Stars = { 8, 21, 34, 47 };
        private static readonly int[] Starts = { 1, 14, 27, 40 };
        private static readonly PlayerColor[] Cols = { PlayerColor.Red, PlayerColor.Green, PlayerColor.Yellow, PlayerColor.Blue };
        private static readonly int[] Bases = { 100, 200, 300, 400 };

        [MenuItem("Tools/ARLudo/Generate Board Tiles")]
        public static void Generate()
        {
            var root = Selection.activeGameObject;
            if (root == null) { EditorUtility.DisplayDialog("ARLudo", "Select board root first.", "OK"); return; }
            float s = 0.04f;
            Undo.RegisterCompleteObjectUndo(root, "Generate Board");

            var pp = MakeChild(root, "PerimeterTiles");
            for (int i = 0; i < 52; i++)
            {
                var go = MakeTile(pp, $"Tile_{i:D2}", Perimeter[i] * s, s);
                var tm = go.AddComponent<TileMarker>();
                tm.tileIndex = i;
                if (System.Array.IndexOf(Stars, i) >= 0)
                { tm.tileType = TileType.SafeStar; go.GetComponent<Renderer>().sharedMaterial = Mat(new Color(1f, 0.95f, 0.4f)); }
                else { int si = System.Array.IndexOf(Starts, i); if (si >= 0)
                { tm.tileType = TileType.StartingTile; tm.ownerColor = Cols[si]; go.GetComponent<Renderer>().sharedMaterial = Mat(BoardReference.GetPlayerColor(Cols[si])); }
                else { tm.tileType = TileType.Normal; go.GetComponent<Renderer>().sharedMaterial = Mat(Color.white); } }
            }

            var cp = MakeChild(root, "HomeCorridors");
            for (int c = 0; c < 4; c++)
                for (int i = 0; i < 6; i++)
                {
                    var go = MakeTile(cp, $"Home_{Cols[c]}_{i}", HomeCor[c][i] * s, s);
                    var tm = go.AddComponent<TileMarker>();
                    tm.tileIndex = Bases[c] + i;
                    tm.tileType = i == 5 ? TileType.Goal : TileType.HomeCorridor;
                    tm.ownerColor = Cols[c];
                    Color col = BoardReference.GetPlayerColor(Cols[c]);
                    if (i == 5) col = Color.Lerp(col, Color.white, 0.4f);
                    go.GetComponent<Renderer>().sharedMaterial = Mat(col);
                }

            var yp = MakeChild(root, "Yards");
            var br = root.GetComponent<BoardReference>() ?? Undo.AddComponent<BoardReference>(root);
            for (int c = 0; c < 4; c++)
            {
                var yg = MakeChild(yp, $"Yard_{Cols[c]}");
                var sl = new Transform[4];
                for (int i = 0; i < 4; i++)
                { var slot = new GameObject($"Slot_{i}"); Undo.RegisterCreatedObjectUndo(slot, "slot");
                  slot.transform.SetParent(yg.transform); slot.transform.localPosition = Yards[c][i] * s; sl[i] = slot.transform; }
                switch (Cols[c]) { case PlayerColor.Red: br.redYardSlots = sl; break; case PlayerColor.Green: br.greenYardSlots = sl; break;
                    case PlayerColor.Yellow: br.yellowYardSlots = sl; break; case PlayerColor.Blue: br.blueYardSlots = sl; break; }
            }
            EditorUtility.SetDirty(root);
        }

        static GameObject MakeChild(GameObject p, string n)
        { var g = new GameObject(n); Undo.RegisterCreatedObjectUndo(g, n); g.transform.SetParent(p.transform); g.transform.localPosition = Vector3.zero; return g; }
        static GameObject MakeTile(GameObject p, string n, Vector3 pos, float sz)
        { var g = GameObject.CreatePrimitive(PrimitiveType.Cube); Undo.RegisterCreatedObjectUndo(g, n); g.name = n;
          g.transform.SetParent(p.transform); g.transform.localPosition = new Vector3(pos.x, 0.003f, pos.z);
          g.transform.localScale = new Vector3(sz * 0.9f, 0.004f, sz * 0.9f); return g; }
        static Material Mat(Color c) { var m = new Material(Shader.Find("Universal Render Pipeline/Lit")); m.color = c; return m; }
    }
}