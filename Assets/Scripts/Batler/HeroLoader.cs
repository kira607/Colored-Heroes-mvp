using UnityEngine;

namespace Batler
{
    public class HeroLoader : MonoBehaviour
    {
        private string _heroName;
        private GameObject _heroPrefab;
        private RectTransform _parentRectTransform;

        public void Init(GameObject heroPrefab, RectTransform parentRectTransform)
        {
            _heroPrefab = heroPrefab;
            _parentRectTransform = parentRectTransform;
            _heroName = "";
        }

        public Hero Load(string heroName)
        {
            _heroName = heroName;
            string info = LoadInfo();
            HeroInfo heroInfo = JsonUtility.FromJson<HeroInfo>(info);

            GameObject newHero = Instantiate(_heroPrefab, _parentRectTransform);
            Hero hero = newHero.GetComponent<Hero>();
            hero.ApplyInfo(heroInfo);
            hero.gameObject.SetActive(false);
            return hero;
        }

        private string LoadInfo()
        {
            var t = Resources.Load<TextAsset>("Heroes/" + _heroName + "/" + _heroName + ".config");
            return t.text;
        }
    }
}