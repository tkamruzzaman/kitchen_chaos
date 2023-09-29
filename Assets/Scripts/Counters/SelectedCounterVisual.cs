using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour
{
    private BaseCounter baseCounter;
    private List<GameObject> visualGameObjectList = new List<GameObject>();

    private void Awake()
    {
        baseCounter = GetComponentInParent<BaseCounter>();

        foreach (Transform child in transform)
        {
            visualGameObjectList.Add(child.gameObject);
        }
    }

    private void Start()
    {
        //Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
        Hide();
    }

    private void OnDestroy()
    {
        //Player.Instance.OnSelectedCounterChanged -= Player_OnSelectedCounterChanged;
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e)
    {
        if (e.selectedCounter == baseCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach (GameObject visualGameObject in visualGameObjectList)
        {
            visualGameObject.SetActive(true);
        }
    }

    private void Hide()
    {
        foreach (GameObject visualGameObject in visualGameObjectList)
        {
            visualGameObject.SetActive(false);
        }
    }
}