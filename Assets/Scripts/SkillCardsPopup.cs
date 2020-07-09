using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine;

public class SkillCardsPopup : PopupBase
{
    [SerializeField]
    private GameObject skillCardPrefab;
    [SerializeField]
    private RectTransform skillCardsRoot;
    [SerializeField]
    private Text title;

    private List<GameObject> skillCards;

    public void SetData(List<SkillBase> skills, string titleText)
    {
        title.text = titleText;
        if (skillCards == null) skillCards = new List<GameObject>();
        foreach (var item in skills)
        {
            GameObject go = Instantiate(skillCardPrefab, skillCardsRoot);
            var script = go.GetComponent<SkillCardView>();
            script.SetData(item);
            skillCards.Add(go);
        }
    }

    public void ClearView()
    {
        if (skillCards.Count > 0)
        {
            foreach (var item in skillCards)
            {
                Destroy(item);
            }
        }
        skillCards.Clear();
    }
}
