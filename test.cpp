#include <iostream>
#include <vector>
#include <string>
#include <cassert>
#include<list>

using namespace std;

enum Face
{
    Front = 1,
    Back,
    Top,
    Bottom,
    Left,
    Right
};

class Quad
{
public:
    Face face;
    int row, col;
    int num = -1;
    Quad(Face face,int row,int col){
        this->face=face;
        this->row=row;
        this->col=col;
    }
};

// 所有的 3x3*6 格子
vector<Quad*> allTheQuads;
vector<vector<Quad*>> edgeNeighbor;//邻边格表
vector<vector<Quad*>> diagonalNeighbor;//对角格表
// 获取指定格子
Quad* GetQuad(Face face, int row, int col)
{
    for (auto q : allTheQuads)
    {
        if (q->face == face && q->row == row && q->col == col)
            return q;
    }
    return nullptr;
}

// 邻边格
Quad* GetEdgeNeighborQuad(Face face, int row, int col, int dr, int dc)
{
    int newRow = row + dr;
    int newCol = col + dc;

    if (newRow >= 1 && newRow <= 3 && newCol >= 1 && newCol <= 3)
        return GetQuad(face, newRow, newCol);

    bool isUp = dr == -1 && dc == 0;
    bool isDown = dr == 1 && dc == 0;
    bool isLeft = dr == 0 && dc == -1;
    bool isRight = dr == 0 && dc == 1;

    Face targetFace = face;
    int targetRow = row, targetCol = col;

    switch (face)
    {
    case Front:
        if (isUp)
        {
            targetFace = Top;
            targetRow = 3;
            targetCol = col;
        }
        if (isDown)
        {
            targetFace = Bottom;
            targetRow = 1;
            targetCol = col;
        }
        if (isLeft)
        {
            targetFace = Left;
            targetRow = row;
            targetCol = 3;
        }
        if (isRight)
        {
            targetFace = Right;
            targetRow = row;
            targetCol = 1;
        }
        break;
    case Back:
        if (isUp)
        {
            targetFace = Bottom;
            targetRow = 3;
            targetCol = col;
            
        }
        if (isDown)
        {
            targetFace = Top;
            targetRow = 1;
            targetCol = col;
        }
        if (isLeft)
        {
            targetFace = Left;
            targetRow = 4-row;
            targetCol = 1;
            
        }
        if (isRight)
        {
            targetFace = Left;
            targetRow = 4-row;
            targetCol = 3;
        }
        break;
    case Top:
        if (isUp)
        {
            targetFace = Back;
            targetRow = 3;
            targetCol = col;
        }
        if (isDown)
        {
            targetFace = Front;
            targetRow = 1;
            targetCol = col;
        }
        if (isLeft)
        {
            targetFace = Left;
            targetRow = 1;
            targetCol = row;
        }
        if (isRight)
        {
            targetFace = Right;
            targetRow = 1;
            targetCol = 4 - row;
        }
        break;
    case Bottom:
        if (isUp)
        {
            targetFace = Front;
            targetRow = 3;
            targetCol = col;
        }
        if (isDown)
        {
            targetFace = Back;
            targetRow = 1;
            targetCol = col;
        }
        if (isLeft)
        {
            targetFace = Left;
            targetRow = 3;
            targetCol = 4 - row;
        }
        if (isRight)
        {
            targetFace = Right;
            targetRow = 3;
            targetCol = row;
        }
        break;
    case Left:
        if (isUp)
        {
            targetFace = Top;
            targetRow = col;
            targetCol = 1;
        }
        if (isDown)
        {
            targetFace = Bottom;
            targetRow = 4 - col;
            targetCol = 1;
        }
        if (isLeft)
        {
            targetFace = Back;
            targetRow = 4-row;
            targetCol = 1;
        }
        if (isRight)
        {
            targetFace = Front;
            targetRow = row;
            targetCol = 1;
        }
        break;
    case Right:
        if (isUp)
        {
            targetFace = Top;
            targetRow = 4 - col;
            targetCol = 3;
        }
        if (isDown)
        {
            targetFace = Bottom;
            targetRow = col;
            targetCol = 3;
        }
        if (isLeft)
        {
            targetFace = Front;
            targetRow = row;
            targetCol = 3;
        }
        if (isRight)
        {
            targetFace = Back;
            targetRow = 4-row;
            targetCol = 3;
        }
        break;
    }

    return GetQuad(targetFace, targetRow, targetCol);
}

// 对角格
Quad *GetDiagonalNeighborQuad(Face face, int row, int col, int dr, int dc)
{
    int midRow = row + dr;
    int midCol = col + dc;

    if (midRow >= 1 && midRow <= 3 && midCol >= 1 && midCol <= 3)
        return GetQuad(face, midRow, midCol);

    bool isLU=dr==-1&&dc==-1;
    bool isRU=dr==-1&&dc==1;
    bool isLD=dr==1&&dc==-1;
    bool isRD=dr==1&&dc==1;
    if(isLU&&row==1&&col==1) return nullptr;
    if(isRU&&row==1&&col==3) return nullptr;
    if(isLD&&row==3&&col==1) return nullptr;
    if(isRD&&row==3&&col==3) return nullptr;

    Quad *middle1 = GetEdgeNeighborQuad(face, row, col, dr, 0);
    Quad *final1 = GetEdgeNeighborQuad(middle1->face, middle1->row, middle1->col, 0, dc);
    return final1;
}

// 初始化全部格子
void InitQuads()
{
    for (int f = 1; f <= 6; ++f)
    {
        for (int r = 1; r <= 3; ++r)
        {
            for (int c = 1; c <= 3; ++c)
            {
                allTheQuads.emplace_back(new Quad((Face)f, r, c));
            }
        }
    }
}

void initNeighbor()
{
    edgeNeighbor.resize(54);
    diagonalNeighbor.resize(54);
    //初始化邻边格表
    int dRow[] = { -1, 1, 0, 0 };
    int dCol[] = { 0, 0, -1, 1 };
    for (auto quad:allTheQuads)
    {
        for (int d = 0; d < 4; d++)
        {
            Quad *neighbor = GetEdgeNeighborQuad(quad->face, quad->row, quad->col, dRow[d], dCol[d]);
            edgeNeighbor[((int)quad->face - 1) * 9 + (quad->row - 1) * 3 + (quad->col - 1)].emplace_back(neighbor);
        }
    }
    //初始化对角格表
    int diagRow[] = { -1, -1, 1, 1 };
    int diagCol[] = { -1, 1, -1, 1 };

    for (auto quad:allTheQuads)
    {
        for (int d = 0; d < 4; d++)
        {
            Quad* neighbor = GetDiagonalNeighborQuad(quad->face, quad->row, quad->col, diagRow[d], diagCol[d]);
            if(neighbor) diagonalNeighbor[((int)quad->face - 1) * 9 + (quad->row - 1) * 3 + (quad->col - 1)].emplace_back(neighbor);
        }
    }
}

#include <fstream>
#include <string>


//得到当前面可加分单元格
vector<Quad*> GetAllGoalAreas(int num, Face face)
{
    vector<Quad*> ans;
    for (int i = 0; i < 9; i++)
    {
        int index = ((int)face - 1) * 9 + i;
        if (allTheQuads[index]->num == -1)
        {
            vector<Quad*> curEdgeNeighbor = edgeNeighbor[index];
            vector<Quad*> curDiagonalNeighbor = diagonalNeighbor[index];
            int edgeSize = curEdgeNeighbor.size();
            int diagonalSize = curDiagonalNeighbor.size();
            for (int i = 0; i < edgeSize + diagonalSize; i++)
            {
                if (i < edgeSize && curEdgeNeighbor[i]->num == num)
                {
                    ans.emplace_back(allTheQuads[index]);
                    break;
                }
                if (i >= edgeSize && curDiagonalNeighbor[i - edgeSize]->num != num&&curDiagonalNeighbor[i - edgeSize]->num!=-1)
                {
                    ans.emplace_back(allTheQuads[index]);
                    break;
                }
            }
        }
    }
    return ans;
}

int main()
{

    InitQuads();

    initNeighbor();

    allTheQuads[4]->num=1;
    vector<Quad*>Goals=GetAllGoalAreas(1,(Face)1);

    ofstream fout("GoalAreas.txt");


    for (auto cell : Goals) {
        fout << " [F:" << (Face)cell->face
                << " R:" << cell->row
                << " C:" << cell->col << "]";
    }
    fout << "\n";


    fout.close();
    cout << "Written: " << "GoalAreas.txt"  << endl;
    return 0;




    // ofstream fout("edge_neighbors.txt");

    // for (int i = 0; i<(int)edgeNeighbor.size(); ++i) {
    //     fout << "Cell " << i << ":";
    //     for (auto neighbor : edgeNeighbor[i]) {
    //         fout << " [F:" << (Face)neighbor->face
    //              << " R:" << neighbor->row
    //              << " C:" << neighbor->col << "]";
    //     }
    //     fout << "\n";
    // }

    // fout.close();
    // cout << "Written: " << "edge_neighbors.txt"  << endl;
    // WriteNeighborTableToFile(diagonalNeighbor, "diagonal_neighbors.txt");
    
}
