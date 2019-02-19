using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WebCamRawImage : MonoBehaviour
{
    public RawImage background;
    public AspectRatioFitter fit;

    bool isCamAvailable = false;
    WebCamTexture backCam;

    Texture defaultBackground;

    public Shader postShader;
    Material postMaterial;

    public bool performBlit = false;

    // Start is called before the first frame update
    void Start()
    {
        postMaterial = new Material(postShader);
        postMaterial.hideFlags = HideFlags.HideAndDontSave;

        defaultBackground = background.texture;
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            backCam = new WebCamTexture(devices[0].name, Screen.width, Screen.height);
            backCam.Play();
            background.texture = backCam;
            isCamAvailable = true;
        }
        else
        {
            isCamAvailable = false;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(isCamAvailable)
        {
            float ratio = (float)backCam.width / (float)backCam.height;
            fit.aspectRatio = ratio;

            float scaleY = backCam.videoVerticallyMirrored ? -1 : 1;
            background.rectTransform.localScale = new Vector3(1f, scaleY, 1f);

            background.rectTransform.localScale = new Vector3(-1f, 1f, 1f);

            int orient = -backCam.videoRotationAngle;
            background.rectTransform.localEulerAngles = new Vector3(0, 0, orient);
            
            //Shader.SetGlobalTexture("webCamImage", background.texture);
            if(!gameObject.GetComponent<ColorKey>().enabled)
            {
                gameObject.GetComponent<ColorKey>().enabled = true;
            }
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {   
        if (performBlit)
            Graphics.Blit(source, destination, postMaterial);
    }
    
}
