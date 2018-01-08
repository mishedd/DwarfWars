﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using SharpNoise;
using SharpNoise.Modules;
using SharpNoise.Builders;

namespace DwarfWars.Library
{
    public class Worlda
    {
        public ITile[,] Map;
        public Player Creator { get; private set; }
        public List<Player> Players { get; private set; }
        private int[,] map;
        private int width, height;
        private bool useRandomSeed;
        private string seed;
        private Random pseudoRandom;
        private int randomFillpercent;

        public Worlda(int width, int height, Player creator)
        {
            GenerateMap();
            this.width = width;
            this.height = height;
            Creator = creator;
        }

        public Worlda(Worlda world)
        {

        }

        public void GenerateMap()
        {
            map = new int[width, height];
            RandomFillMap();

            for (int i = 0; i < 5; i++)
            {
                SmoothMap();
            }

            GenerateDirt();
            double abundance = 1.5;

            for (int o = 0; o < 5; o++)
            {
                for (int i = 0; i < height * width * abundance / 1024; i++)
                {
                    GenerateVeins((float)abundance, o + 3);
                }
                abundance -= .2;
            }
        }

        private void GenerateDirt()
        {
            NoiseMap output = new NoiseMap();
            Perlin per = new Perlin() { Seed = seed.GetHashCode(), Frequency = 1, OctaveCount = Perlin.DefaultOctaveCount, Lacunarity = Perlin.DefaultLacunarity, Persistence = Perlin.DefaultPersistence, Quality = NoiseQuality.Best };
            var noiseMapBuilder = new PlaneNoiseMapBuilder() { DestNoiseMap = output, SourceModule = per };

            noiseMapBuilder.SetDestSize(width, height);
            noiseMapBuilder.SetBounds(-2, 3, -2, 3);
            noiseMapBuilder.Build();

            for (int r = 0; r < width; r++)
            {
                for (int c = 0; c < height; c++)
                {
                    if (map[r, c] != 0)
                    {
                        map[r, c] = (Math.Abs(output[r, c]) * 100) > 80 ? 2 : map[r, c];
                    }
                }
            }
        }

        private void GenerateVeins(float abundance, int tileNum)
        {

            Vector2 veinPos = new Vector2(pseudoRandom.Next(0, width - 1), pseudoRandom.Next(0, height - 1));
            int veinLength = (int)(pseudoRandom.NextDouble() * pseudoRandom.NextDouble() * 75 * abundance);

            for (int i = 0; i < veinLength; i++)
            {
                veinPos.X += pseudoRandom.Next(-1, 2);
                veinPos.Y += pseudoRandom.Next(-1, 2);

                if (IsInMapRange((int)veinPos.X, (int)veinPos.Y) && map[(int)veinPos.X, (int)veinPos.Y] != 0)
                {
                    map[(int)veinPos.X, (int)veinPos.Y] = tileNum;
                }
            }
        }

        private void RandomFillMap()
        {
            if (useRandomSeed)
            {
                seed = DateTime.Now.ToString();
                useRandomSeed = false;
            }

            pseudoRandom = new Random(seed.GetHashCode());

            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    if (x == 0 || x == width - 1 || y == 0 || y == height - 1)
                    {
                        map[x, y] = 1;
                    }
                    else
                    {
                        int temp = pseudoRandom.Next(0, 100);
                        map[x, y] = (temp < randomFillpercent) ? 1 : 0;
                    }
                }
            }
        }

        private void SmoothMap()
        {
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    int neighbourWallTiles = GetSurroundingWallCount(x, y);
                    if (neighbourWallTiles > 4)
                        map[x, y] = 1;
                    if (neighbourWallTiles < 4)
                        map[x, y] = 0;


                }
            }
        }

        private int GetSurroundingWallCount(int gridX, int gridY)
        {
            int wallCount = 0;
            for (int neighbourX = gridX - 1; neighbourX <= gridX + 1; neighbourX++)
            {
                for (int neighbourY = gridY - 1; neighbourY <= gridY + 1; neighbourY++)
                {
                    if (neighbourX >= 0 && neighbourX < width && neighbourY >= 0 && neighbourY < height)
                    {
                        if (neighbourX != gridX || neighbourY != gridY)
                        {
                            wallCount += map[neighbourX, neighbourY];
                        }
                    }
                    else
                    {
                        wallCount++;
                    }
                }
            }

            return wallCount;
        }

        private bool IsInMapRange(int x, int y)
        {
            return x >= 0 && x < width && y >= 0 && y < height;
        }


    }

    public abstract class IWorld
    {
        public GameState GameState;
        public Player Creator;

        public IWorld(GameState gameState, Player creator)
        {
            GameState = gameState;
            Creator = creator;
        }
    }

    public class Lobby : IWorld
    {
        List<Player> Players;

        public Lobby(List<Player> players, Player creator) : base(GameState.Lobby, creator)
        {
            Players = players;
        }
    }

    public class Game : IWorld
    {
        ITile[,] Map;
        Team[] Teams;

        public Game(ITile[,] map, Team[] teams, Player creator) : base(GameState.Game, creator)
        {
            Map = map;
            Teams = teams;
        }
    }

    public class PostGame : IWorld
    {
        Team Winner;

        public PostGame(Team winner, Player creator) : base(GameState.PostGame, creator)
        {
            Winner = winner;
        }
    }

    public enum GameState
    {
        Lobby, Game, PostGame
    }
}