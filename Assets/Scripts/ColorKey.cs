using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ColorKey : MonoBehaviour
{
    [Range(0.001f, 1f)]
    public float bThreshold;
    [Range(0.001f, 1f)]
    public float rThreshold;
    [Range(0.001f, 1f)]
    public float gThreshold;
    public ComputeShader compute;
    public Shader postShader;

    Material postMaterial;

    RenderTexture imageRender;
    int kernel;

    bool isWebCamAvailable = false;
    WebCamTexture webCamTex;

    RawImage webCamRawImage;
    AspectRatioFitter fit;

    // Start is called before the first frame update
    void Start()
    {
        webCamRawImage = gameObject.GetComponent<WebCamRawImage>().background;

        kernel = compute.FindKernel("CSMain");
        postMaterial = new Material(postShader);
        postMaterial.hideFlags = HideFlags.HideAndDontSave;

        //postMaterial.SetTexture("webCamImage", webCamRawImage.texture);
        Shader.SetGlobalTexture("webCamImage", webCamRawImage.texture);
    }

    private void OnApplicationQuit()
    {
        try
        {
            webCamTex.Stop();
        }
        catch { }
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalTexture("webCamImage", webCamRawImage.texture);
    }

    //private void OnRenderImage(RenderTexture source, RenderTexture destination)
    //{
    //    Graphics.Blit(source, destination, postMaterial);
    //}

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Debug.Log(source.width + " :: " + source.height);
        if (!imageRender)
        {
            Debug.Log("Dimensions dont match");
            imageRender = new RenderTexture(source.width, source.height, 0, RenderTextureFormat.ARGB32);
            imageRender.enableRandomWrite = true;
            imageRender.wrapMode = TextureWrapMode.Clamp;
            imageRender.Create();
        }
        Graphics.Blit(source, imageRender);

        compute.SetFloat("rThreshold", rThreshold);
        compute.SetFloat("gThreshold", gThreshold);
        compute.SetFloat("bThreshold", bThreshold);
        try
        {
            compute.SetTexture(kernel, "webCamImage", Shader.GetGlobalTexture("webCamImage"));
        }
        catch { }
        compute.SetTexture(kernel, "renderImage", imageRender);
        compute.Dispatch(kernel, imageRender.width / 8, imageRender.height / 8, 1);

        Graphics.Blit(source, destination, postMaterial);
    }
}
