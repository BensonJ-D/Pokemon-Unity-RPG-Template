using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using PokemonScripts;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class CsvToSo
    {
        private const string PokemonCsvPath = "/Editor/CSVs/Pokemon.csv";
        private const string MoveListsCsvPath = "/Editor/CSVs/PokemonMoves.csv";
        private const string MovesCsvPath = "/Editor/CSVs/Moves.csv";


        [MenuItem("Utilities/Generate Pokemon")]
        public static void GeneratePokemon()
        {
            Dictionary<int, List<LearnableMove>> movesMap = GenerateMoveLists();

            List<string> allLines = File.ReadAllLines(Application.dataPath + PokemonCsvPath).ToList();
            allLines.RemoveAt(0);
            List<Sprite> frontSprites = AssetDatabase
                .LoadAllAssetsAtPath("Assets/VFX/Sprites/Pokemon/Front/Pokemon_Gen1_Front.png")
                .OfType<Sprite>().ToList();
            List<Sprite> backSprites = AssetDatabase
                .LoadAllAssetsAtPath("Assets/VFX/Sprites/Pokemon/Back/Pokemon_Gen1_Back.png")
                .OfType<Sprite>().ToList();

            foreach (var s in allLines)
            {
                List<string> splitString = s.Split(',').ToList();
                var number = int.Parse(splitString[0]);

                PokemonBase pokemon = ScriptableObject.CreateInstance<PokemonBase>();
            
                var name = splitString[1];
                PokemonType type1 = (PokemonType) Enum.Parse(typeof(PokemonType), splitString[2]);
                PokemonType type2 = (PokemonType) Enum.Parse(typeof(PokemonType), splitString[3]);
                Sprite front = frontSprites.Find(sprite => sprite.name == $"Pokemon_Gen1_Front_{number - 1}");
                Sprite back = backSprites.Find(sprite => sprite.name == $"Pokemon_Gen1_Back_{number - 1}");
                var maxHp = int.Parse(splitString[4]);
                var attack = int.Parse(splitString[5]);
                var defence = int.Parse(splitString[6]);
                var spAttack = int.Parse(splitString[7]);
                var spDefence = int.Parse(splitString[8]);
                var speed = int.Parse(splitString[9]);

                List<LearnableMove> learnableMoves = movesMap[number];

                pokemon.InitialiseInstance(front, back, number, name, type1, type2, 
                    maxHp, attack, defence, spAttack, spDefence, speed, learnableMoves);

                AssetDatabase.CreateAsset(pokemon, $"Assets/Game/Resources/Pokemon/{pokemon.Number}_{pokemon.Species}.asset");
            }

            AssetDatabase.SaveAssets();
        }

        private static Dictionary<int, List<LearnableMove>> GenerateMoveLists()
        {
            List<string> allMovesGUIDs =
                AssetDatabase.FindAssets("t:MoveBase", new[] {"Assets/Game/Resources/Moves"}).ToList();
            List<string> allMoves = new List<string>();
            foreach (var guid in allMovesGUIDs)
            {
                allMoves.Add(AssetDatabase.GUIDToAssetPath(guid));
            }

            List<string> allLines = File.ReadAllLines(Application.dataPath + MoveListsCsvPath).ToList();
            allLines.RemoveAt(0);

            Dictionary<int, List<LearnableMove>> movesMap = new Dictionary<int, List<LearnableMove>>();

            foreach (var s in allLines)
            {
                List<string> splitString = s.Split(',').ToList();
                var number = int.Parse(splitString[0]);
                var move = int.Parse(splitString[1]);
                // var how = int.Parse(splitString[2]);
                var level = int.Parse(splitString[3]);

                if (!movesMap.ContainsKey(number))
                {
                    movesMap.Add(number, new List<LearnableMove>());
                }

                var moveNameRegex = new Regex($"^.*/{move}_.*\\.asset");
                List<string> matchedMoves = allMoves.Where(m => moveNameRegex.IsMatch(m)).ToList();
                MoveBase moveBase = AssetDatabase.LoadAssetAtPath(matchedMoves[0], typeof(MoveBase)) as MoveBase;
                LearnableMove learnableMove = new LearnableMove(moveBase, level);

                movesMap[number].Add(learnableMove);
            }

            return movesMap;
        }

        [MenuItem("Utilities/Generate Moves")]
        public static void GenerateMoves()
        {
            List<string> allLines = File.ReadAllLines(Application.dataPath + MovesCsvPath).ToList();
            allLines.RemoveAt(0);

            foreach (var s in allLines)
            {
                List<string> splitString = s.Split(',').ToList();

                MoveBase move = ScriptableObject.CreateInstance<MoveBase>();
                var number = int.Parse(splitString[0]);
                var name = splitString[1];
                PokemonType type = (PokemonType) int.Parse(splitString[2]);
                int power;
                try {
                    power = int.Parse(splitString[3]);
                } catch (Exception e) {
                    power = 0;
                }

                var pp = int.Parse(splitString[4]);
                int accuracy;
                try {
                    accuracy = int.Parse(splitString[5]);
                } catch (Exception e) {
                    accuracy = 0;
                }

                MoveCategory moveCategory = (MoveCategory) int.Parse(splitString[8]);

                
                move.InitialiseInstance(number, name, type, power, pp, accuracy, moveCategory, new List<EffectType>(), MoveTarget.Foe, 0);

                AssetDatabase.CreateAsset(move, $"Assets/Game/Resources/Moves/{move.Number}_{move.Name}.asset");
            }

            AssetDatabase.SaveAssets();
        }
    }
}