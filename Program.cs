using System;
namespace MiniGame
{

    public class QuadHelper
    {
        public static QuadHelper instance;

        static QuadHelper()
        {
            if (instance == null)
                instance = new QuadHelper();
        }

        public enum Face
        {
            Front = 1,
            Back,
            Top,
            Bottom,
            Left,
            Right,

        };


        public class Quad : MonoBehaviour
        {
            /// <summary>
            /// 朝向
            /// </summary>
            public Face face;
            [Header("行")]
            public int row;
            [Header("列")]
            public int col;
            [Header("安放的数字")]
            public int num = -1;
            public Color originColor;
            private void Awake()
            {
                this.originColor = this.GetComponent<MeshRenderer>().material.color;
            }

            public void SetColor(Color color)
            {
                this.GetComponent<MeshRenderer>().material.color = color;
            }

            public void ResetColor()
            {
                this.GetComponent<MeshRenderer>().material.color = originColor;
            }

            public void SetNum(int newNum)
            {
                if (this.num == -1)
                {
                    this.num = newNum;
                    //通知后端计算得分
                }
                else
                    return;

            }

        };
        List<List<Quad>> edgeNeighbor;//邻边格表
        List<List<Quad>> diagonalNeighbor;//对角格表

        //寻找对应单元格
        public Quad GetQuad(List<Quad> allTheQuads, Face targetFace, int targetRow, int targetCol)
        {
            foreach (Quad it in allTheQuads)
            {
                if (it.face == targetFace && it.row == targetRow && it.col == targetCol) return it;
            }
            return null;
        }

        //邻边格计算
        public Quad GetEdgeNeighborQuad(Face face, int row, int col, int dr, int dc, List<Quad> allTheQuads)
        {
            int newRow = row + dr;
            int newCol = col + dc;

            // 当前面内有效
            if (newRow >= 1 && newRow <= 3 && newCol >= 1 && newCol <= 3)
                foreach (Quad it in allTheQuads)
                {
                    if (it.face == face && it.row == newRow && it.col == newCol) return it;
                }

            // 方向判断
            bool isUp = (dr == -1 && dc == 0);
            bool isDown = (dr == 1 && dc == 0);
            bool isLeft = (dr == 0 && dc == -1);
            bool isRight = (dr == 0 && dc == 1);

            // 映射目标坐标
            Face targetFace = face;
            int targetRow = row, targetCol = col;

            switch (face)
            {
                case Face.Front:
                    if (isUp) { targetFace = Face.Top; targetRow = 3; targetCol = col; }
                    if (isDown) { targetFace = Face.Bottom; targetRow = 1; targetCol = col; }
                    if (isLeft) { targetFace = Face.Left; targetRow = row; targetCol = 3; }
                    if (isRight) { targetFace = Face.Right; targetRow = row; targetCol = 1; }
                    break;

                case Face.Back:
                    if (isUp) { targetFace = Face.Bottom; targetRow = 3; targetCol = col; }
                    if (isDown) { targetFace = Face.Top; targetRow = 1; targetCol = col; }
                    if (isLeft) { targetFace = Face.Left; targetRow = 4 - row; targetCol = 1; }
                    if (isRight) { targetFace = Face.Right; targetRow = 4 - row; targetCol = 3; }
                    break;

                case Face.Top:
                    if (isUp) { targetFace = Face.Back; targetRow = 3; targetCol = col; }
                    if (isDown) { targetFace = Face.Front; targetRow = 1; targetCol = col; }
                    if (isLeft) { targetFace = Face.Left; targetRow = 1; targetCol = row; }
                    if (isRight) { targetFace = Face.Right; targetRow = 1; targetCol = 4 - row; }
                    break;

                case Face.Bottom:
                    if (isUp) { targetFace = Face.Front; targetRow = 3; targetCol = col; }
                    if (isDown) { targetFace = Face.Back; targetRow = 1; targetCol = col; }
                    if (isLeft) { targetFace = Face.Left; targetRow = 3; targetCol = 4 - row; }
                    if (isRight) { targetFace = Face.Right; targetRow = 3; targetCol = row; }
                    break;

                case Face.Left:
                    if (isUp) { targetFace = Face.Top; targetRow = col; targetCol = 1; }
                    if (isDown) { targetFace = Face.Bottom; targetRow = 4 - col; targetCol = 1; }
                    if (isLeft) { targetFace = Face.Back; targetRow = 4 - row; targetCol = 1; }
                    if (isRight) { targetFace = Face.Front; targetRow = row; targetCol = 1; }
                    break;

                case Face.Right:
                    if (isUp) { targetFace = Face.Top; targetRow = 4 - col; targetCol = 3; }
                    if (isDown) { targetFace = Face.Bottom; targetRow = col; targetCol = 3; }
                    if (isLeft) { targetFace = Face.Front; targetRow = row; targetCol = 3; }
                    if (isRight) { targetFace = Face.Back; targetRow = 4 - row; targetCol = 3; }
                    break;
            }
            Quad ans = GetQuad(allTheQuads, targetFace, targetRow, targetCol);
            return ans;
        }

        //对角格计算
        public Quad GetDiagonalNeighborQuad(Face face, int row, int col, int dr, int dc, List<Quad> allTheQuads)
        {
            int midRow = row + dr;
            int midCol = col + dc;

            // 情况一：同一面上
            if (midRow >= 1 && midRow <= 3 && midCol >= 1 && midCol <= 3)
                return GetQuad(allTheQuads, face, midRow, midCol);

            if (midRow >= 1 && midRow <= 3 && midCol >= 1 && midCol <= 3)
                return GetQuad(allTheQuads, face, midRow, midCol);

            bool isLU = dr == -1 && dc == -1;
            bool isRU = dr == -1 && dc == 1;
            bool isLD = dr == 1 && dc == -1;
            bool isRD = dr == 1 && dc == 1;
            if (isLU && row == 1 && col == 1) return null;
            if (isRU && row == 1 && col == 3) return null;
            if (isLD && row == 3 && col == 1) return null;
            if (isRD && row == 3 && col == 3) return null;

            // 情况二：越界跨面
            Quad middle1 = GetEdgeNeighborQuad(face, row, col, dr, 0, allTheQuads);
            Quad final1 = GetEdgeNeighborQuad(middle1.face, middle1.row, middle1.col, 0, dc, allTheQuads);
            return GetQuad(allTheQuads, final1.face, final1.row, final1.col); ;
        }

        //得到邻边格表和对角格表
        public void initNeighbor(List<Quad> allTheQuads)
        {
            edgeNeighbor = new List<List<Quad>>(54);
            for (int i = 0; i < 54; i++)
            {
                edgeNeighbor.Add(new List<Quad>());
            }
            diagonalNeighbor = new List<List<Quad>>(54);
            for (int i = 0; i < 54; i++)
            {
                diagonalNeighbor.Add(new List<Quad>());
            }

            //初始化邻边格表
            int[] dRow = { -1, 1, 0, 0 };
            int[] dCol = { 0, 0, -1, 1 };
            foreach (Quad quad in allTheQuads)
            {
                for (int d = 0; d < 4; d++)
                {
                    Quad neighbor = GetEdgeNeighborQuad(quad.face, quad.row, quad.col, dRow[d], dCol[d], allTheQuads);
                    edgeNeighbor[((int)quad.face - 1) * 9 + (quad.row - 1) * 3 + (quad.col - 1)].Add(neighbor);
                }
            }
            //初始化对角格表
            int[] diagRow = { -1, -1, 1, 1 };
            int[] diagCol = { -1, 1, -1, 1 };

            foreach (Quad quad in allTheQuads)
            {
                for (int d = 0; d < 4; d++)
                {
                    Quad neighbor = GetDiagonalNeighborQuad(quad.face, quad.row, quad.col, diagRow[d], diagCol[d], allTheQuads);
                    if (neighbor) diagonalNeighbor[((int)quad.face - 1) * 9 + (quad.row - 1) * 3 + (quad.col - 1)].Add(neighbor);
                }
            }
        }

        //访问邻边格表
        public List<Quad> VisitEdgeNeighbor(Quad curQuad, List<Quad> allTheQuads)
        {
            int f = (int)curQuad.face;
            int r = curQuad.row;
            int c = curQuad.col;
            return edgeNeighbor[(f - 1) * 9 + (r - 1) * 3 + c - 1];
        }

        //访问对角格表
        public List<Quad> VisitDiagonalNeighbor(Quad curQuad, List<Quad> allTheQuads)
        {
            int f = (int)curQuad.face;
            int r = curQuad.row;
            int c = curQuad.col;
            return diagonalNeighbor[(f - 1) * 9 + (r - 1) * 3 + c - 1];
        }

        //计算加分
        public int GetGoals(Quad curQuad)
        {
            int f = (int)curQuad.face;
            int r = curQuad.row;
            int c = curQuad.col;
            int num = curQuad.num;
            List<Quad> curEdgeNeighbor = edgeNeighbor[(f - 1) * 9 + (r - 1) * 3 + c - 1];
            List<Quad> curDiagonalNeighbor = diagonalNeighbor[(f - 1) * 9 + (r - 1) * 3 + c - 1];
            int ans = 0;
            //邻边相同加分
            foreach (Quad quad in curEdgeNeighbor)
            {
                ans += (quad.num == num && num != -1) ? 1 : 0;
            }
            //对角不同加分
            foreach (Quad quad in curDiagonalNeighbor)
            {
                ans += (quad.num != num && quad.num != -1) ? 1 : 0;
            }
            return ans;
        }
        //得到当前面可加分单元格
        public List<Quad> GetAllGoalAreas(int num, Face face, List<Quad> allTheQuads)
        {
            List<Quad> ans = new List<Quad>();
            for (int i = 0; i < 9; i++)
            {
                int index = ((int)face - 1) * 9 + i;
                if (allTheQuads[index].num == -1)
                {
                    List<Quad> curEdgeNeighbor = edgeNeighbor[index];
                    List<Quad> curDiagonalNeighbor = diagonalNeighbor[index];
                    int edgeSize = curEdgeNeighbor.size();
                    int diagonalSize = curDiagonalNeighbor.size();
                    for (int i = 0; i < edgeSize + diagonalSize; i++)
                    {
                        if (i < edgeSize && curEdgeNeighbor[i].num == num)
                        {
                            ans.Add(curEdgeNeighbor[i]);
                            break;
                        }
                        if (i >= edgeSize && curDiagonalNeighbor[i - edgeSize].num == num)
                        {
                            ans.Add(curDiagonalNeighbor[i - edgeSize]);
                            break;
                        }
                    }
                }
            }
            return ans;
        }
    }

}