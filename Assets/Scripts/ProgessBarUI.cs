using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private Image progessBarImage;
    private CuttingCounter cuttingCounter;

    private void Awake()
    {
        cuttingCounter = GetComponentInParent<CuttingCounter>();
    }

    private void Start()
    {
        cuttingCounter.OnProgessChanged += CuttingCounter_OnProgessChanged;
        
        progessBarImage.fillAmount = 0;

        Hide();
    }

    private void CuttingCounter_OnProgessChanged(object sender, CuttingCounter.OnProgessChangedEventArgs e)
    {
        progessBarImage.fillAmount = e.progessNormalized;
        if (e.progessNormalized == 0 || e.progessNormalized == 1)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
