using UnityEngine;

public static class UIHelpers
{    
    //definitions
    private static Camera mainCam; 

    //singleton to load main camera reference ONCE 
    public static Camera MainCam
    {
        get 
        {
            if (mainCam == null) { mainCam = Camera.main;}
            //can also do: mainCam ??= Camera.main;
            return mainCam; 
        }
    }
    
    public static void FaceElementToCamera(RectTransform rect)
    {
        rect.rotation = MainCam.transform.rotation; 
    }

}

