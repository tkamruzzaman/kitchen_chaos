using UnityEngine;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour
{
    [SerializeField] private Image progessBarImage;
    private IHasProgress hasProgress;

    private void Awake()
    {
        hasProgress = GetComponentInParent<IHasProgress>();
        if (hasProgress == null)
        {
            Debug.LogError("Doesn't contain IHasProgress interface");
        }
    }

    private void Start()
    {
        hasProgress.OnProgessChanged += HasProgress_OnProgessChanged;
        
        progessBarImage.fillAmount = 0;

        Hide();
    }

    private void HasProgress_OnProgessChanged(object sender, IHasProgress.OnProgessChangedEventArgs e)
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
