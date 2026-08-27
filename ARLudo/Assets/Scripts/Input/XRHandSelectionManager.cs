using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using ARLudo.Visuals;

public class XRHandSelectionManager : MonoBehaviour
{
    void OnEnable()
    {
        UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor[] interactors = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>(true);
        foreach (UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor interactor in interactors)
        {
            interactor.selectEntered.AddListener(OnPawnSelected);
        }
    }

    void OnDisable()
    {
        UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor[] interactors = FindObjectsOfType<UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor>(true);
        foreach (UnityEngine.XR.Interaction.Toolkit.Interactors.XRRayInteractor interactor in interactors)
        {
            interactor.selectEntered.RemoveListener(OnPawnSelected);
        }
    }

    private void OnPawnSelected(SelectEnterEventArgs args)
    {
        GameObject selectedObject = args.interactableObject.transform.gameObject;
        
        PawnVisual pawn = selectedObject.GetComponent<PawnVisual>();
        if (pawn != null)
        {
            GameBootstrapper bootstrapper = FindObjectOfType<GameBootstrapper>();
            if (bootstrapper != null)
            {
                bootstrapper.OnPawnSelected(pawn);
            }
        }
    }
    
}