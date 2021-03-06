﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessTables
{
    partial class Moves
    {
        const int SlideStrategy = 1;
        const ulong onA = 0x0101010101010101;

        IEnumerable<FigureMove> NextRookMove(FigureCoord figureCoord)
        {
            switch (SlideStrategy)
            {
                case 1: return NextRookMove_Steps(figureCoord);
                case 2:
               default: return NextRookMove_BitMask(figureCoord);
            }
        }



        IEnumerable<FigureMove> NextRookMove_Steps (FigureCoord figureCoord)
        {
            ulong rookMoves = AllRookSteps(figureCoord);
            rookMoves.ulong2ascii().Print();
            foreach (Coord to in Bitboard.NextCoord(rookMoves))
                yield return new FigureMove(figureCoord, to);
        }

        ulong AllRookSteps(FigureCoord figureCoord)
        {
            figureCoord.coord.extract(out int x, out int y);
            ColorType color = figureCoord.figure.GetColor();
            ulong stops = board.GetOwnedBits();
            return
                (Step6(x, y, stops) |
                 Step4(x, y, stops) |
                 Step8(x, y, stops) |
                 Step2(x, y, stops)) & ~board.GetColorBits(color);
        }

        ulong Step1(int x, int y, ulong stops) => Step(x, y,-1,-1, stops);
        ulong Step2(int x, int y, ulong stops) => Step(x, y, 0,-1, stops);
        ulong Step3(int x, int y, ulong stops) => Step(x, y,+1,-1, stops);
        ulong Step4(int x, int y, ulong stops) => Step(x, y,-1, 0, stops);
        ulong Step6(int x, int y, ulong stops) => Step(x, y,+1, 0, stops);
        ulong Step7(int x, int y, ulong stops) => Step(x, y,-1, 1, stops);
        ulong Step8(int x, int y, ulong stops) => Step(x, y, 0, 1, stops);
        ulong Step9(int x, int y, ulong stops) => Step(x, y,+1, 1, stops);

        ulong Step(int x, int y, int sx, int sy, ulong stops)
        {
            ulong map = 0;
            while (true)
            {
                x += sx;
                y += sy;
                if (!Coord.OnBoard (x, y)) 
                    break;
                ulong bit = 1UL << ((y << 3) | x);
                map |= bit;
                if (0 != (stops & bit))
                    break;
            }
            return map;
        }




        IEnumerable<FigureMove> NextRookMove_BitMask(FigureCoord figureCoord)
        {
            ulong rookMoves = AllRookSlides(figureCoord);
            rookMoves.ulong2ascii().Print();
            foreach (Coord to in Bitboard.NextCoord(rookMoves))
                yield return new FigureMove(figureCoord, to);
        }

        ulong AllRookSlides(FigureCoord figureCoord)
        {
            ulong rook = figureCoord.coord.GetBit();
            ColorType color = figureCoord.figure.GetColor();
            ulong stops = board.GetOwnedBits();
            return
                (Slide6(rook, stops) |
                 Slide4(rook, stops) |
                 Slide8(rook, stops) |
                 Slide2(rook, stops)) & ~board.GetColorBits(color);
        }

        ulong Slide1(ulong coord, ulong stops) => Slide(coord, -9, noH, stops);
        ulong Slide2(ulong coord, ulong stops) => Slide(coord, -8, ALL, stops);
        ulong Slide3(ulong coord, ulong stops) => Slide(coord, -7, noA, stops);
        ulong Slide4(ulong coord, ulong stops) => Slide(coord, -1, noH, stops);
        ulong Slide6(ulong coord, ulong stops) => Slide(coord, +1, noA, stops);
        ulong Slide7(ulong coord, ulong stops) => Slide(coord, +7, noH, stops);
        ulong Slide8(ulong coord, ulong stops) => Slide(coord, +8, ALL, stops);
        ulong Slide9(ulong coord, ulong stops) => Slide(coord, +9, noA, stops);

        ulong Slide (ulong coord, int shift, ulong range, ulong stops)
        {
            ulong map = 0UL;
            while (true)
            {
                if (shift > 0)  coord <<= +shift;
                else            coord >>= -shift;
                if (0 == (coord & range)) break;
                map |= coord;
                if (0 != (coord & stops)) break;
            }
            return map;
        }


    }
}
