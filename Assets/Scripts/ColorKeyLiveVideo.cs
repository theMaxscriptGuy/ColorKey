using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorKeyLiveVideo : MonoBehaviour
{
    [System.Serializable]
    public struct ColorThreshold
    {
        [Range(-180.000f, 180.000f)]
        public float R;
        [Range(-255.000f, 255.000f)]
        public float G;
        [Range(-255.000f, 255.000f)]
        public float B;

        public bool uniformThreshold;
        [Range(-1.000f, 1.000f)]
        public float thresholdValue;
    }

    [System.Serializable]
    public struct Resolution
    {
        public int width;
        public int height;
    }

    public Resolution resolution;
    public Shader postShader;
    public ComputeShader computeColorKey;
    [SerializeField]
    public ColorThreshold RGBThreshold;
    public Color keyColor = Color.white;

    private Material m_postMaterial;
    private int m_kernel;
    private WebCamTexture m_webCamTex;
    private RenderTexture m_sourceTexture;

    // Start is called before the first frame update
    void Start()
    {
        if(!InitializeVideoFeed())
        {
            return;
        }
        //Video feed initialized

        if(postShader)
        {
            m_postMaterial = new Material(postShader);
            m_postMaterial.hideFlags = HideFlags.HideAndDontSave;
        }

        if(computeColorKey)
        {
            m_kernel = computeColorKey.FindKernel("CSMain");
        }
    }

    // Update is called once per frame
    void Update()
    {
        Shader.SetGlobalTexture("webCamImage", m_webCamTex);
    }

    private void ColorKeyOut()
    {

    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if(!m_postMaterial)
        {
            return;
        }

        if (!m_sourceTexture)
        {
            CreateRenderTexture(ref m_sourceTexture, source);
        }
        Graphics.Blit(source, m_sourceTexture);

        computeColorKey.SetVector("keyColor", keyColor);

        if(RGBThreshold.uniformThreshold)
        {
            RGBThreshold.R = RGBThreshold.G = RGBThreshold.B = RGBThreshold.thresholdValue;
        }

        computeColorKey.SetFloat("rThreshold", RGBThreshold.R);
        computeColorKey.SetFloat("gThreshold", RGBThreshold.G);
        computeColorKey.SetFloat("bThreshold", RGBThreshold.B);

        computeColorKey.SetTexture(m_kernel, "renderImage", m_sourceTexture);
        computeColorKey.SetTexture(m_kernel, "camVideoFeed", Shader.GetGlobalTexture("webCamImage"));
        computeColorKey.Dispatch(m_kernel, source.width / 8, source.height / 8, 1);

        Graphics.Blit(m_sourceTexture, destination, m_postMaterial);
    }

    private void OnApplicationQuit()
    {
        try
        {
            m_webCamTex.Stop();
        }
        catch { }
    }


    private bool InitializeVideoFeed()
    {
        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            m_webCamTex = new WebCamTexture(devices[0].name, Screen.width, Screen.height);
            m_webCamTex.Play();
            //gameObject.GetComponent<Renderer>().material.mainTexture = webCamTex;
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CreateRenderTexture(ref RenderTexture texture, RenderTexture source)
    {
        Debug.Log("Dimensions dont match");
        m_sourceTexture = new RenderTexture(resolution.width,resolution.height, 0, RenderTextureFormat.ARGB32);
        m_sourceTexture.enableRandomWrite = true;
        m_sourceTexture.wrapMode = TextureWrapMode.Clamp;
        m_sourceTexture.Create();
    }
}
