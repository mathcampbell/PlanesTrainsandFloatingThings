using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class UnderwaterController : MonoBehaviour
{
    public Color underwaterFogColorShallow;
    public Color underwaterFogColorDeep;
    public Color underwaterLightColor;
    public Color underwaterDeepLightColor;
    public Color daylightColor;
    private bool isUnderwater;
    public WaterController CurrentWaterController;
    private Camera TheCamera;
    private Volume UnderwaterPost;
    public Light TheSun;
    public Blit BlitPass;
    private float originalSunIntensity = 1.3f;
    private float originalSunBounceIntensity = 1f;
    private float currentDepth;
    
    public Skybox theSkybox;
    public Material SkyboxMat;

    // Start is called before the first frame update
    void Start()
    {
        TheCamera = GetComponentInChildren<Camera>();
        UnderwaterPost = GetComponentInChildren<Volume>();
      //  TheSun.intensity = originalSunIntensity;
      //  TheSun.bounceIntensity = originalSunBounceIntensity;
       // theSkybox = GetComponentInChildren<Skybox>();
        //SkyboxMat = theSkybox.material;
        
        
    }

    // Update is called once per frame
    void Update()
    {
        var currentPosition = TheCamera.transform.position;
        currentDepth = CurrentWaterController.DistanceToWater(currentPosition, Time.time);
        if (currentDepth <0)
        {
            var depthLevel = Mathf.Abs(currentDepth) / 20f;
           // TheSun.color = Color.Lerp(daylightColor, underwaterLightColor, depthLevel);
          //  TheSun.intensity = Mathf.Lerp(originalSunIntensity, .1f, (Mathf.Abs(currentDepth)/50f));
            Shader.SetGlobalColor("_FogColor", Color.Lerp(underwaterFogColorShallow, underwaterFogColorDeep, depthLevel));
            if (isUnderwater == false)
            GoUnderwater();
            
        }
       else
            if (isUnderwater == true)
        {
            GoSurface();
        }
    }

    void  GoUnderwater()
    {
        var depthAbs = Mathf.Abs(currentDepth);
        isUnderwater = true;
        //RenderSettings.fog = true;
        var depthLevel = depthAbs / 20f;
        RenderSettings.fogColor = Color.Lerp(underwaterFogColorShallow, underwaterFogColorDeep, depthLevel);
        RenderSettings.fogDensity = Mathf.Lerp(0.01f, 0.8f, depthLevel);
        UnderwaterPost.enabled = true;

        
        //TheSun.bounceIntensity = Mathf.Lerp(originalSunBounceIntensity, 0f, depthLevel);
        //TheCamera.backgroundColor = Color.Lerp(underwaterFogColorShallow, underwaterFogColorDeep, depthLevel);
        //TheCamera.clearFlags = CameraClearFlags.SolidColor;
        
        BlitPass.PassEnabled(true);
    }

    void GoSurface()
    {
        isUnderwater = false;
        RenderSettings.fog = false;
        UnderwaterPost.enabled = false;
        // TheSun.intensity = originalSunIntensity;
        //  TheSun.bounceIntensity = originalSunBounceIntensity;
      //  TheCamera.clearFlags = CameraClearFlags.Skybox;
      //     TheSun.color = daylightColor;
        BlitPass.PassEnabled(false);
    }
}
