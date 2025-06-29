using UnityEngine;
using Rewired;

public class RewiredMapCategorySwitcher : MonoBehaviour
{
    #region Serialized Fields
    [SerializeField] private int m_PlayerId = 0;
    [SerializeField, HideInInspector] public int m_MapCategoryIndex = 0;
    [SerializeField, HideInInspector] private string m_MapCategory = "Default";
    #endregion

    #region Private Fields
    private Player m_Player;
    #endregion

    #region Unity Lifecycle
    private void Start()
    {
        m_Player = ReInput.players.GetPlayer(m_PlayerId);
        SetActiveMapCategory(m_MapCategoryIndex);
    }

    #endregion

    #region Public Methods
    public void SetActiveMapCategory(int categoryIndex)
    {
        if (m_Player == null) return;
        var categories = ReInput.mapping.MapCategories;
        if (categoryIndex < 0 || categoryIndex >= categories.Count) return;
        int categoryId = categories[categoryIndex].id;
        // Wyłącz wszystkie mapy
        foreach (var map in m_Player.controllers.maps.GetAllMaps())
        {
            map.enabled = false;
        }
        // Włącz wybraną kategorię
        foreach (var map in m_Player.controllers.maps.GetAllMaps())
        {
            if (map.categoryId == categoryId)
            {
                map.enabled = true;
            }
        }
    }

    // Pozwala edytorowi ustawić kategorię
    public void SetCategoryByIndex(int index, string name)
    {
        m_MapCategoryIndex = index;
        m_MapCategory = name;
    }
    #endregion
} 