using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Net.HungryBug.Atb.Game
{
    public class GameManager : MonoBehaviour
    {
        private MapReader mapSpawner;
        private List<ScriptableUnit> _units;
        public BaseHero SelectedHero;

        public List<Tile> heroesTiles;
        public List<Tile> enemiesTiles;

        public void SpawnHeroes()
        {
            var heroCount = 1;

            for (int i = 0; i < heroCount; i++)
            {
                var randomPrefab = GetRandomUnit<BaseHero>(Faction.Hero);
                var spawnedHero = Instantiate(randomPrefab);
                var randomSpawnTile = GridManager.Instance.GetHeroSpawnTile();

                randomSpawnTile.SetUnit(spawnedHero);
            }

           // GameManager.Instance.ChangeState(GameState.SpawnEnemies);
        }

        public void SpawnEnemies()
        {
            var enemyCount = 1;

            for (int i = 0; i < enemyCount; i++)
            {
                var randomPrefab = GetRandomUnit<BaseEnemy>(Faction.Enemy);
                var spawnedEnemy = Instantiate(randomPrefab);
                var randomSpawnTile = GridManager.Instance.GetEnemySpawnTile();

                randomSpawnTile.SetUnit(spawnedEnemy);
            }

           // GameManager.Instance.ChangeState(GameState.HeroesTurn);
        }

        private T GetRandomUnit<T>(Faction faction) where T : BaseUnit
        {
            return (T)_units.Where(u => u.Faction == faction).OrderBy(o => Random.value).First().UnitPrefab;
        }

        public void SetSelectedHero(BaseHero hero)
        {
            SelectedHero = hero;
            MenuManager.Instance.ShowSelectedHero(hero);
        }
    }

}
