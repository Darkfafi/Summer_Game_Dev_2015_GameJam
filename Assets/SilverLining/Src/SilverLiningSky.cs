// Copyright (c) 2011-2012 Sundog Software LLC. All rights reserved worldwide.

using UnityEngine;
using System;
using System.Collections.Generic;

public class SilverLiningSky
{
    private double PI = 3.14159265;
    private float PIf = 3.14159265f;
    protected double DEGREES (double x)
    {
        return x * (180.0 / PI);
    }
    protected double RADIANS (double x)
    {
        return x * (PI / 180.0);
    }
    protected float RADIANS (float x)
    {
        return x * (PIf / 180.0f);
    }

    protected double NITS (double irradiance)
    {
        return irradiance * 683.0 / 3.14;
    }

    public float XBoost = 0, YBoost = 0, ZBoost = 0;
    public int boostExp = 3;
    public float sunTransmissionScale = 1.0f, sunScatteredScale = 1.0f;
    public float moonTransmissionScale = 1.0f, moonScatteredScale = 1.0f;
    public double aScale = 1.0, bScale = 1.0, cScale = 1.0, dScale = 1.0, eScale = 1.0;
    public double moonScale = 0.01;
    public double sunLuminanceScale = 1.0, moonLuminanceScale = 0.1;
    private double H = 8435.0;
    public float oneOverGamma = 0.45f;
    public float sunDistance = 90000.0f;
    public float sunWidthDegrees = 1.0f;
    public float moonDistance = 90000.0f;
    public float moonWidthDegrees = 1.0f;
    public float duskZenithLuminance = 0.02f;
    public float fogThickness = 500.0f;
    public int sphereSegments = 32;

    public SilverLiningSky ()
    {
        sunDistance *= (float)SilverLining.unitScale;
        moonDistance *= (float)SilverLining.unitScale;
        H *= (float)SilverLining.unitScale;

        ephemeris = new SilverLiningEphemeris ();
        InitTwilightLuminances ();
        sunSpectrum = new SilverLiningSolarSpectrum ();
        lunarSpectrum = new SilverLiningLunarSpectrum ();
        XYZ2RGB = new SilverLiningMatrix3 (3.240479, -0.969256, 0.055648, -1.537150, 1.875992, -0.204043, -0.498535, 0.041556, 1.057311);

        XYZ2RGB4 = new Matrix4x4 ();
        XYZ2RGB4[0, 0] = 3.240479f;
        XYZ2RGB4[0, 1] = -0.969256f;
        XYZ2RGB4[0, 2] = 0.055648f;
        XYZ2RGB4[0, 3] = 0.0f;
        XYZ2RGB4[1, 0] = -1.537150f;
        XYZ2RGB4[1, 1] = 1.875992f;
        XYZ2RGB4[1, 2] = -0.204043f;
        XYZ2RGB4[1, 3] = 0.0f;
        XYZ2RGB4[2, 0] = -0.498535f;
        XYZ2RGB4[2, 1] = 0.041556f;
        XYZ2RGB4[2, 2] = 1.057311f;
        XYZ2RGB4[2, 3] = 0.0f;
        XYZ2RGB4[3, 0] = 0.0f;
        XYZ2RGB4[3, 1] = 0.0f;
        XYZ2RGB4[3, 2] = 0.0f;
        XYZ2RGB4[3, 3] = 1.0f;
    }

    public void Start ()
    {
        sun = GameObject.Find ("SilverLiningSun");
        Transform sunTrans = sun.GetComponent<Transform> ();
        sunTrans.position = new Vector3 (sunDistance, 0.0f, 0.0f);
        ParticleEmitter emit = sun.GetComponent<ParticleEmitter> ();
        Particle[] ps = new Particle[1];
        const float sunDiscPer = (256.0f - (57.0f * 2.0f)) / 256.0f;
        float sunDiscSize = sunWidthDegrees * (1.0f / sunDiscPer);
        ps[0].size = sunDistance * (float)Math.Tan (RADIANS (sunDiscSize * 0.5f)) * 2.0f;
        ps[0].energy = 1.0f;
        emit.particles = ps;

        moon = GameObject.Find ("SilverLiningMoon");
        Transform moonTrans = moon.GetComponent<Transform> ();
        moonTrans.position = new Vector3 (moonDistance, 0.0f, 0.0f);
        emit = moon.GetComponent<ParticleEmitter> ();
        ps = new Particle[1];
        const float moonDiscPer = (256.0f - (3.0f * 2.0f)) / 256.0f;
        float moonDiscSize = moonWidthDegrees * (1.0f / moonDiscPer);
        ps[0].size = moonDistance * (float)Math.Tan (RADIANS (moonDiscSize * 0.5f)) * 2.0f;
        ps[0].energy = 1.0f;
        emit.particles = ps;

        starFogShader = Shader.Find("Particles/Stars");
        starNoFogShader = Shader.Find("Particles/Stars No Fog");

        moonTextures = new Texture[30];
        for (int i = 0; i < 30; i++) {
            String texName = "moonday" + (i + 1);
            moonTextures[i] = (Texture)Resources.Load (texName);
        }

        ParticleRenderer ren = moon.GetComponent<ParticleRenderer> ();
        ren.material.mainTexture = moonTextures[15];
		ren.enabled = true;
		
		ren = sun.GetComponent<ParticleRenderer> ();
		ren.enabled = true;
        
        sunLight = GameObject.Find ("SilverLiningSunLight");
        moonLight = GameObject.Find ("SilverLiningMoonLight");
        
        stars = new SilverLiningStars (ephemeris);

        createSphere();
    }

    public void Update (SilverLiningTime time, SilverLiningLocation loc, Renderer renderer, bool bIsOvercast,
        bool doFog)
    {
        ephemeris.Update (time, loc);

        isOvercast = bIsOvercast;

        lightingChanged = false;
        ComputeSun (loc.GetAltitude ());
        ComputeMoon (loc.GetAltitude ());
        
        UpdatePerezCoefficients ();
        UpdateZenith (loc.GetAltitude ());
        
        sunx = Perezx (0, thetaS);
        suny = Perezy (0, thetaS);
        sunY = PerezY (0, thetaS);
        moonY = PerezY (0, thetaM);
        moonx = Perezx (0, thetaM);
        moony = Perezy (0, thetaM);
        
        ComputeLogAvg ();
        ComputeToneMappedSkyLight ();
        
        renderer.material.SetColor ("theColor", Color.cyan);
        
        Vector4 sunPerez = new Vector4 ((float)sunx, (float)suny, (float)sunY, 1.0f);
        Vector4 zenithPerez = new Vector4 ((float)xZenith, (float)yZenith, (float)YZenith, 1.0f);
        Vector4 moonPerez = new Vector4 ((float)moonx, (float)moony, (float)moonY, 1.0f);
        Vector4 zenithMoonPerez = new Vector4 ((float)xMoon, (float)yMoon, (float)YMoon, 1.0f);
        
        Vector4 xPerezABC = new Vector4 ((float)Ax, (float)Bx, (float)Cx, 1.0f);
        Vector4 xPerezDE = new Vector4 ((float)Dx, (float)Ex, 0.0f, 1.0f);
        Vector4 yPerezABC = new Vector4 ((float)Ay, (float)By, (float)Cy, 1.0f);
        Vector4 yPerezDE = new Vector4 ((float)Dy, (float)Ey, 0, 0);
        Vector4 YPerezABC = new Vector4 ((float)AY, (float)BY, (float)CY, 1.0f);
        Vector4 YPerezDE = new Vector4 ((float)DY, (float)EY, 0, 0);
        
        double sfRod, sfCone;
        SilverLiningLuminanceMapper.GetLuminanceScales (out sfRod, out sfCone);
        Vector4 luminanceScales = new Vector4 ((float)sfRod, (float)sfCone, 0, 0);
        
        Vector4 kAndLdmax = new Vector4 ((float)SilverLiningLuminanceMapper.GetRodConeBlend (), (float)SilverLiningLuminanceMapper.GetMaxDisplayLuminance (), oneOverGamma, 1.0f);
        
        Vector3 sunPos = ephemeris.GetSunPositionHorizon ();
        sunPos.Normalize ();
        
        Vector3 moonPos = ephemeris.GetMoonPositionHorizon ();
        moonPos.Normalize ();

        Vector4 overcast = new Vector4 (isOvercast ? 1 : 0, isOvercast ? overcastBlend : 0, overcastTransmission, 0.0f);

        Color fogColor = new Color (1.0f, 1.0f, 1.0f, 1.0f);
        float fogDensity = 0;
        float fogDistance = 1E20f;
        
        if (RenderSettings.fog && doFog) {
            fogColor = RenderSettings.fogColor;
            fogDensity = RenderSettings.fogDensity;
            fogDistance = fogThickness;
        }

        Vector4 fog = new Vector4 (fogColor.r, fogColor.g, fogColor.b, fogDensity);
        overcast.w = fogDistance;

        renderer.material.SetVector ("sunPos", sunPos);
        renderer.material.SetVector ("moonPos", moonPos);
        renderer.material.SetVector ("sunPerez", sunPerez);
        renderer.material.SetVector ("moonPerez", moonPerez);
        renderer.material.SetVector ("zenithMoonPerez", zenithMoonPerez);
        renderer.material.SetVector ("zenithPerez", zenithPerez);

        renderer.material.SetVector ("xPerezABC", xPerezABC);
        renderer.material.SetVector ("xPerezDE", xPerezDE);
        renderer.material.SetVector ("yPerezABC", yPerezABC);
        renderer.material.SetVector ("yPerezDE", yPerezDE);
        renderer.material.SetVector ("YPerezABC", YPerezABC);
        renderer.material.SetVector ("YPerezDE", YPerezDE);
        renderer.material.SetVector ("luminanceScales", luminanceScales);
        renderer.material.SetVector ("kAndLdmax", kAndLdmax);
        renderer.material.SetVector ("overcast", overcast);
        renderer.material.SetVector ("fog", fog);
        renderer.material.SetMatrix ("XYZtoRGB", XYZ2RGB4);

        UpdateSun ();
        UpdateMoon ();
        UpdateLight ();
        if (YZenith < duskZenithLuminance) {
            stars.Enable (true);
            stars.Update ();
        } else {
            stars.Enable (false);
        }

        if (starFogShader != null && starNoFogShader != null) {
            ParticleRenderer ren = sun.GetComponent<ParticleRenderer>();
            if (ren != null) ren.material.shader = doFog ? starFogShader : starNoFogShader;
            ren = moon.GetComponent<ParticleRenderer>();
            if (ren != null) ren.material.shader = doFog ? starFogShader : starNoFogShader;
            stars.SetShader(doFog ? starFogShader : starNoFogShader);
        }
    }

    private void createSphere()
    {
        int i, j, ntri, nvec;

        nvec = 2 + sphereSegments * sphereSegments * 2;
        ntri = (sphereSegments * 4 + sphereSegments * 4 * (sphereSegments - 1));

        GameObject sky = GameObject.Find("_SilverLiningSky");
        if (sky == null) {
            Debug.LogError("_SilverLiningSky not found!");
            return;
        }
        MeshFilter meshFilter = sky.GetComponent<MeshFilter>();
        if (meshFilter==null){
            Debug.LogError("MeshFilter not found!");
            return;
        }
        Mesh mesh = meshFilter.sharedMesh;
        if (mesh == null){
            meshFilter.mesh = new Mesh();
            mesh = meshFilter.sharedMesh;
        }
        mesh.Clear();

        Vector3[] verts = new Vector3[nvec];
        int[] triangles = new int[ntri * 3];

        float dj = (float)Math.PI / ((float)sphereSegments + 1.0f);
        float di = (float)Math.PI / (float)sphereSegments;

        verts[0] = new Vector3(0, 1, 0);
        verts[1] = new Vector3(0, -1, 0);

        for (j=0; j < sphereSegments; j++) {
            for (i=0; i < sphereSegments * 2; i++) {
                float y = (float)Math.Cos((j + 1) * dj);
                float x = (float)Math.Sin(i * di) * (float)Math.Sin((j + 1) * dj);
                float z = (float)Math.Cos(i * di) * (float)Math.Sin((j + 1) * dj);
                verts[2+i+j*sphereSegments*2] = new Vector3(x, y, z);
            }
        }

        for (i=0; i < sphereSegments * 2; i++) {
            triangles[3*i] = 0;
            triangles[3*i+1] = i + 2;
            triangles[3*i+2] = i + 3;
            if (i == sphereSegments * 2 - 1) {
                triangles[3*i+2] = 2;
            }
        }

        int v;
        int ind;
        for (j = 0; j < sphereSegments - 1; j++) {
            v = 2+j*sphereSegments*2;
            ind = 3*sphereSegments*2 + j*6*sphereSegments*2;
            for (i = 0; i < sphereSegments * 2; i++) {
                triangles[6*i+ind] = v+i;
                triangles[6*i+2+ind] = v+i+1;
                triangles[6*i+1+ind] = v+i+sphereSegments*2;
                triangles[6*i+ind+3] = v+i+sphereSegments*2;
                triangles[6*i+2+ind+3] = v+i+1;
                triangles[6*i+1+ind+3] = v+i+sphereSegments*2+1;
                if (i == sphereSegments*2-1) {
                    triangles[6*i+2+ind] = v+i+1-2*sphereSegments;
                    triangles[6*i+2+ind+3] = v+i+1-2*sphereSegments;
                    triangles[6*i+1+ind+3] = v+i+sphereSegments*2+1-2*sphereSegments;
                }
            }
        }

        v = nvec - sphereSegments*2;
        ind = ntri*3 - 3*sphereSegments*2;
        for (i=0; i < sphereSegments*2; i++) {
            triangles[3*i+ind] = 1;
            triangles[3*i+1+ind] = v+i+1;
            triangles[3*i+2+ind] = v+i;
            if (i==sphereSegments*2-1) {
                triangles[3*i+1+ind] = v;
            }
        }

        mesh.vertices = verts;
        mesh.triangles = triangles;
        mesh.RecalculateBounds();
        mesh.Optimize();
    }

    private static void scaleDownToOne (ref Vector3 v)
    {
        float minC = 0.0f;
        if (v.x < minC)
            minC = v.x;
        if (v.y < minC)
            minC = v.y;
        if (v.z < minC)
            minC = v.z;
        v.x -= minC;
        v.y -= minC;
        v.z -= minC;
        
        float maxC = v.x;
        if (v.y > maxC)
            maxC = v.y;
        if (v.z > maxC)
            maxC = v.z;
        
        if (maxC > 1.0f) {
            v.x /= maxC;
            v.y /= maxC;
            v.z /= maxC;
        }
    }

    private void UpdateLight ()
    {
        const float epsilon = 1E-7f;
        Vector3 sunXYZ = sunTransmittedLuminance * (float)sunLuminanceScale;
        
        SilverLiningLuminanceMapper.DurandMapperXYZ (ref sunXYZ);
        
        Vector3 rgb = sunXYZ * XYZ2RGB;
        
        ApplyGamma (ref rgb);
        
        Color sunLightColor = new Color (rgb.x, rgb.y, rgb.z);
        
        Transform trans = sunLight.GetComponent<Transform> ();
        Vector3 sunPos = trans.position;
        trans.LookAt (sunPos - ephemeris.GetSunPositionHorizon ());
        
        Light light = sunLight.GetComponent<Light> ();
        light.color = sunLightColor * sunLightScale;
        light.enabled = light.color.g > epsilon;
        
        Vector3 moonXYZ = moonTransmittedLuminance * (float)moonLuminanceScale;
        
        SilverLiningLuminanceMapper.DurandMapperXYZ (ref moonXYZ);
        
        rgb = moonXYZ * XYZ2RGB;
        
        ApplyGamma (ref rgb);
        
        Color moonLightColor = new Color (rgb.x, rgb.y, rgb.z);
        
        trans = moonLight.GetComponent<Transform> ();
        Vector3 moonPos = trans.position;
        trans.LookAt (moonPos - ephemeris.GetMoonPositionHorizon ());
        
        light = moonLight.GetComponent<Light> ();
        light.color = moonLightColor * moonLightScale;
        light.enabled = light.color.g > epsilon;
        
        int moonDay = (int)(Math.Floor ((ephemeris.GetMoonPhaseAngle () / (2.0 * PI)) * 30.0));
        ParticleRenderer ren = moon.GetComponent<ParticleRenderer> ();
        ren.material.mainTexture = moonTextures[moonDay];
		ren.enabled = true;
        
        RenderSettings.ambientLight = skyLight * ambientLightScale;
    }

    private void UpdateMoon ()
    {
        Vector3 moonPosHorizon = ephemeris.GetMoonPositionHorizon ();
        moonPosHorizon.Normalize ();
        moonPosHorizon *= moonDistance;
        
        Transform trans = moon.GetComponent<Transform> ();
        trans.position = moonPosHorizon + Camera.main.transform.position;
        
        Vector3 moonXYZ = moonTransmittedLuminance;
        Vector3 moonColorV = moonXYZ * XYZ2RGB;
        scaleDownToOne (ref moonColorV);
        
        moonColorV.Normalize ();
        
        if (moonColorV.x < 0)
            moonColorV.x = 0;
        if (moonColorV.y < 0)
            moonColorV.y = 0;
        if (moonColorV.z < 0)
            moonColorV.z = 0;
        
        ApplyGamma (ref moonColorV);
        
        Vector3 one = new Vector3 (1, 1, 1);
        moonColorV = moonColorV * (float)isothermalEffect + one * (1.0f - (float)isothermalEffect);
        
        Color moonColor = new Color (moonColorV.x, moonColorV.y, moonColorV.z);
        
        ParticleEmitter e = moon.GetComponent<ParticleEmitter> ();
        Particle[] particles = e.particles;
        particles[0].color = moonColor;
        e.particles = particles;
    }

    private void UpdateSun ()
    {
        Vector3 sunPosHorizon = ephemeris.GetSunPositionHorizon ();
        sunPosHorizon.Normalize ();
        sunPosHorizon *= sunDistance;
        
        Transform trans = sun.GetComponent<Transform> ();
        trans.position = sunPosHorizon + Camera.main.transform.position;
        
        Vector3 sunXYZ = sunTransmittedLuminance;
        Vector3 sunColorV = sunXYZ * XYZ2RGB;
        scaleDownToOne (ref sunColorV);
        
        sunColorV.Normalize ();
        
        if (sunColorV.x < 0)
            sunColorV.x = 0;
        if (sunColorV.y < 0)
            sunColorV.y = 0;
        if (sunColorV.z < 0)
            sunColorV.z = 0;
        
        ApplyGamma (ref sunColorV);
        
        Vector3 one = new Vector3 (1, 1, 1);
        sunColorV = sunColorV * (float)isothermalEffect + one * (1.0f - (float)isothermalEffect);
        
        Color sunColor = new Color (sunColorV.x, sunColorV.y, sunColorV.z);
        
        ParticleEmitter e = sun.GetComponent<ParticleEmitter> ();
        Particle[] particles = e.particles;
        particles[0].color = sunColor;
        e.particles = particles;
    }

    private void ComputeLogAvg ()
    {
        double Y, R;
        Vector3 scatteredLuminance;
        
        scatteredLuminance = sunScatteredLuminance + moonScatteredLuminance + moonTransmittedLuminance;
        
        scatteredLuminance.y += (float)NightSkyLuminance () * 1000.0f;
        
        Y = scatteredLuminance.y;
        R = -0.702 * scatteredLuminance.x + 1.039 * scatteredLuminance.y + 0.433 * scatteredLuminance.z;
        
        SilverLiningLuminanceMapper.SetSceneLogAvg (R, Y);
    }

    private void ComputeToneMappedSkyLight ()
    {
        Vector3 sunXYZ = sunScatteredLuminance * (float)sunLuminanceScale;
        sunXYZ.y += (float)NightSkyLuminance () * 1000.0f;
        
        Vector3 moonXYZ = moonScatteredLuminance * (float)moonLuminanceScale;
        
                /*
        if (Atmosphere::GetHDREnabled())
        {
            // convert to kCd
            sunXYZ = sunXYZ * 0.001;
            moonXYZ = moonXYZ * 0.001;
        }
        else */
        {
            SilverLiningLuminanceMapper.DurandMapperXYZ (ref sunXYZ);
            SilverLiningLuminanceMapper.DurandMapperXYZ (ref moonXYZ);
        }
        
        Vector3 XYZ = (sunXYZ + moonXYZ);
        
//      if (!Atmosphere::GetHDREnabled())
        {
            if (XYZ.x > (float)maxSkylightLuminance)
                XYZ.x = (float)maxSkylightLuminance;
            if (XYZ.y > (float)maxSkylightLuminance)
                XYZ.y = (float)maxSkylightLuminance;
            if (XYZ.z > (float)maxSkylightLuminance)
                XYZ.z = (float)maxSkylightLuminance;
        }
        
        Vector3 rgb = XYZ * XYZ2RGB;
        
        //scaleDownToOne(rgb);
        ApplyGamma (ref rgb);
        
        skyLight = new Color (rgb.y, rgb.y, rgb.y);
    }

    private void ApplyGamma (ref Vector3 v)
    {
        float min = 0;
        if (v.x < min)
            min = v.x;
        if (v.y < min)
            min = v.y;
        if (v.z < min)
            min = v.z;
        min = -min;
        
        v.x = v.x + min;
        v.y = v.y + min;
        v.z = v.z + min;
        
        //if (!Atmosphere::GetHDREnabled())
        {
            float max = v.x;
            if (v.y > max)
                max = v.y;
            if (v.z > max)
                max = v.z;
            if (max > 1.0f) {
                v.x /= max;
                v.y /= max;
                v.z /= max;
            }
            
            if (v.x > 0.0f)
                v.x = (float)Math.Pow (v.x, oneOverGamma);
            if (v.y > 0.0f)
                v.y = (float)Math.Pow (v.y, oneOverGamma);
            if (v.z > 0.0f)
                v.z = (float)Math.Pow (v.z, oneOverGamma);
        }
    }

    private double PerezY (double theta, double gamma)
    {
        return (1.0 + AY * Math.Exp (BY / Math.Cos (theta))) * (1.0 + CY * Math.Exp (DY * gamma) + EY * Math.Cos (gamma) * Math.Cos (gamma));
    }

    private double Perezx (double theta, double gamma)
    {
        return (1.0 + Ax * Math.Exp (Bx / Math.Cos (theta))) * (1.0 + Cx * Math.Exp (Dx * gamma) + Ex * Math.Cos (gamma) * Math.Cos (gamma));
        
    }

    private double Perezy (double theta, double gamma)
    {
        return (1.0 + Ay * Math.Exp (By / Math.Cos (theta))) * (1.0 + Cy * Math.Exp (Dy * gamma) + Ey * Math.Cos (gamma) * Math.Cos (gamma));
    }

    private double AngleBetween (Vector3 v1, Vector3 v2)
    {
        Vector3 a = v1;
        a.Normalize ();
        Vector3 b = v2;
        b.Normalize ();
        
        double dot = Vector3.Dot (a, b);
        
        return Math.Acos (dot);
    }

    private double NightSkyLuminance ()
    {
        // Bright planets
        // Zodiacal light
        // Integrated starlight
        // Airglow
        // Diffuse galactic light
        double Wm2 = lightPollution + 2.0E-6 + 1.2E-7 + 3.0E-8 + 5.1E-8 + 9.1E-9 + 9.1E-10;
        // Cosmic light
        double nits = NITS (Wm2);
        
        return nits * isothermalEffect * 0.001;
    }

    private void UpdateZenith (double altitude)
    {
        Vector3 sunPos = ephemeris.GetSunPositionHorizon ();
        Vector3 zenithPos = new Vector3 (0, 1, 0);
        thetaS = AngleBetween (zenithPos, sunPos);
        
        Vector3 moonPos = ephemeris.GetMoonPositionHorizon ();
        thetaM = AngleBetween (zenithPos, moonPos);
        
        double den = sunScatteredLuminance.x + sunScatteredLuminance.y + sunScatteredLuminance.z;
        double denMoon = moonScatteredLuminance.x + moonScatteredLuminance.y + moonScatteredLuminance.z;
        
        xZenith = yZenith = xMoon = yMoon = 0.2;
        
        if (den != 0.0) {
            xZenith = sunScatteredLuminance.x / den;
            yZenith = sunScatteredLuminance.y / den;
        }
        
                /*
            // Zenith chromaticity from "A Practical Analytic Model for Daylight"
            double theta3 = thetaS * thetaS * thetaS;
            double theta2 = thetaS * thetaS;
            double T2 = T * T;
            xZenith =
           ( 0.00165 * theta3 - 0.00375 * theta2 + 0.00209 * thetaS + 0.0)     * T2 +
           (-0.02903 * theta3 + 0.06377 * theta2 - 0.03202 * thetaS + 0.00394) * T +
           ( 0.11693 * theta3 - 0.21196 * theta2 + 0.06052 * thetaS + 0.25886);
    
             yZenith =
           ( 0.00275 * theta3 - 0.00610 * theta2 + 0.00317 * thetaS + 0.0)     * T2 +
           (-0.04214 * theta3 + 0.08970 * theta2 - 0.04153 * thetaS + 0.00516) * T +
           ( 0.15346 * theta3 - 0.26756 * theta2 + 0.06670 * thetaS + 0.26688);
         */

        if (denMoon != 0.0) {
            xMoon = moonScatteredLuminance.x / denMoon;
            yMoon = moonScatteredLuminance.y / denMoon;
        }
        
        YMoon = moonScatteredLuminance.y * 0.001 * moonScale;
        
        // Assume that our own scattered sunlight calculation is the zenith luminance.
        YZenith = sunScatteredLuminance.y * 0.001 + NightSkyLuminance ();
        
        // Account for high altitude. As you lose atmosphere, less scattering occurs.
        H *= SilverLining.unitScale;
        
        isothermalEffect = Math.Exp (-(altitude / H));
        if (isothermalEffect < 0)
            isothermalEffect = 0;
        if (isothermalEffect > 1.0)
            isothermalEffect = 1.0;
        YZenith *= isothermalEffect;
        YMoon *= isothermalEffect;
        
        // Alternate approaches:
        
        // Zenith luminance from "A Practical Analytic Model for Daylight" (Preeham, Shirley Smits)
        // double chiSun = (4.0/9.0 - T / 120.0) * (PI - 2 * thetaS);
        // YZenith = (4.053 * T - 4.9710) * tan(chiSun) - 0.2155 * T + 2.4192 + NightSkyLuminance();
        
        // Zenith luminance from "Sky Luminance Distribution Model for Simulation of Daylit
        // Environment" (Igawa, Nakamura, Matsuura)
    }
        /*
           double Ys = (PI * 0.5) - thetaS;
    
           double A = 18.373 * Ys + 9.955;
           double B = -52.013 * Ys - 37.766;
           double C = 46.572 * Ys + 59.352;
           double D = 1.691 * Ys * Ys - 16.498 * Ys - 48.670;
           double E = 1.124 * Ys + 19.738;
           double F = 1.170 * log(Ys) + 6.369;
    
           const double N = 1.0; // normalized global illuminance
           YZenith = exp(A * N * N * N * N * N +
                B * N * N * N * N +
             C * N * N * N +
             D * N * N +
             E * N +
             F) * 0.001 + NightSkyLuminance(); */        
        
    
    private void UpdatePerezCoefficients ()
    {
        if (T != lastT) {
            lastT = T;
            
            AY = (0.1787 * T - 1.4630) * aScale;
            BY = (-0.3554 * T + 0.4275) * bScale;
            CY = (-0.0227 * T + 5.3251) * cScale;
            DY = (0.1206 * T - 2.5771) * dScale;
            EY = (-0.0670 * T + 0.3702) * eScale;
            
            Ax = -0.0193 * T - 0.2592;
            Bx = -0.0665 * T + 0.0008;
            Cx = -0.0004 * T + 0.2125;
            Dx = -0.0641 * T - 0.8989;
            Ex = -0.0033 * T + 0.0452;
            
            Ay = -0.0167 * T - 0.2608;
            By = -0.0950 * T + 0.0092;
            Cy = -0.0079 * T + 0.2102;
            Dy = -0.0441 * T - 1.6537;
            Ey = -0.0109 * T + 0.0529;
        }
    }

    private void ComputeSun (double altitude)
    {
        Vector3 sunPos = ephemeris.GetSunPositionHorizon ();
        sunPos.Normalize ();
        double cosZenith = sunPos.y;
        
        if (lastSunT != T || lastSunZenith != cosZenith) {
            lastSunT = T;
            lastSunZenith = cosZenith;
            
            lightingChanged = true;
            
            if (cosZenith > 0) {
                double zenithAngle = Math.Acos (cosZenith);
                
                SilverLiningSpectrum solarDirect = new SilverLiningSpectrum ();
                SilverLiningSpectrum solarScattered = new SilverLiningSpectrum ();
                
                sunSpectrum.ApplyAtmosphericTransmittance (zenithAngle, cosZenith, T, altitude, ref solarDirect, ref solarScattered);
                
                sunTransmittedLuminance = solarDirect.ToXYZ ();
                sunScatteredLuminance = solarScattered.ToXYZ ();
                
                // Apply sunset color tweaks
                double alpha = zenithAngle / (PI * 0.5);
                for (int i = 0; i < boostExp; i++)
                    alpha *= alpha;
                sunScatteredLuminance.x *= (float)(1.0 + alpha * XBoost);
                sunScatteredLuminance.y *= (float)(1.0 + alpha * YBoost);
                sunScatteredLuminance.z *= (float)(1.0 + alpha * ZBoost);
            } else {
                // In twilight conditions, we lookup luminance based on experimental results
                // on cloudless nights.
                float solarAltitude = (float)DEGREES (Math.Asin (sunPos.y));
                
                int lower = (int)Math.Floor (solarAltitude);
                int higher = (int)Math.Ceiling (solarAltitude);
                
                float alpha = (float)(solarAltitude - lower);
                
                float a = 0;
                float b = 0;
                if (lower >= -16 && higher >= -16) {
                    a = twilightLuminance[lower];
                    b = twilightLuminance[higher];
                }
                
                // Blend light from sunset
                const double epsilon = 0.001;
                SilverLiningSpectrum solarDirect = new SilverLiningSpectrum ();
                SilverLiningSpectrum solarScattered = new SilverLiningSpectrum ();
                
                double zenithAngle = PI * 0.5 - epsilon;
                sunSpectrum.ApplyAtmosphericTransmittance (zenithAngle, Math.Cos (zenithAngle), T, altitude, ref solarDirect, ref solarScattered);
                sunTransmittedLuminance = solarDirect.ToXYZ ();
                sunScatteredLuminance = solarScattered.ToXYZ ();
                
                float Y = (1 - alpha) * a + alpha * b;
                // luminance per lookup table
                float x = 0.25f;
                float y = 0.25f;
                float minDirectional = 0.1f;
                
                float X = x * (Y / y);
                float Z = (1.0f - x - y) * (Y / y);
                
                alpha = -solarAltitude / 2.0f;
                if (alpha > 1.0f)
                    alpha = 1.0f;
                if (alpha < 0)
                    alpha = 0;
                alpha = alpha * alpha;
                sunTransmittedLuminance = sunTransmittedLuminance * Y * minDirectional * alpha + sunTransmittedLuminance * (1.0f - alpha);
                Vector3 twilight = new Vector3 (X, Y, Z);
                sunScatteredLuminance = twilight * alpha + sunScatteredLuminance * (1.0f - alpha);
                
                // Apply sunset color tweaks
                sunScatteredLuminance.x *= 1.0f + XBoost;
                sunScatteredLuminance.y *= 1.0f + YBoost;
                sunScatteredLuminance.z *= 1.0f + ZBoost;
            }
            
            if (isOvercast) {
                sunTransmittedLuminance = (sunTransmittedLuminance * (overcastBlend * overcastTransmission)) + (sunTransmittedLuminance * (1.0f - overcastBlend));
                sunScatteredLuminance = (sunScatteredLuminance * (overcastBlend * overcastTransmission)) + (sunScatteredLuminance * (1.0f - overcastBlend));
            }
            
            
            sunTransmittedLuminance = sunTransmittedLuminance * sunTransmissionScale;
            sunScatteredLuminance = sunScatteredLuminance * sunScatteredScale;
        }
    }

    private void ComputeMoon (double altitude)
    {
        Vector3 moonPos = ephemeris.GetMoonPositionHorizon ();
        moonPos.Normalize ();
        
        double cosZenith = moonPos.y;
        double zenith = Math.Acos (cosZenith);
        
        if (lastMoonT != T || lastMoonZenith != zenith) {
            lastMoonT = T;
            lastMoonZenith = zenith;
            
            lightingChanged = true;
            
            SilverLiningSpectrum moonSpectrumEarthDirect = new SilverLiningLunarSpectrum ();
            SilverLiningSpectrum moonSpectrumEarthScattered = new SilverLiningLunarSpectrum ();
            
            lunarSpectrum.ApplyAtmosphericTransmittance (zenith, cosZenith, T, altitude, ref moonSpectrumEarthDirect, ref moonSpectrumEarthScattered);
            float moonLuminance = MoonLuminance ();
            moonTransmittedLuminance = moonSpectrumEarthDirect.ToXYZ () * moonLuminance;
            moonScatteredLuminance = moonSpectrumEarthScattered.ToXYZ () * moonLuminance;
            
            if (isOvercast) {
                moonTransmittedLuminance = (moonTransmittedLuminance * (overcastBlend * overcastTransmission)) + (moonTransmittedLuminance * (1.0f - overcastBlend));
                moonScatteredLuminance = (moonScatteredLuminance * (overcastBlend * overcastTransmission)) + (moonScatteredLuminance * (1.0f - overcastBlend));
            }
            
            moonTransmittedLuminance = moonTransmittedLuminance * moonTransmissionScale;
            moonScatteredLuminance = moonScatteredLuminance * moonScatteredScale;
            
        }
    }

    private float MoonLuminance ()
    {
        float luminance = 0;
        
        if (ephemeris != null) {
            Vector3 moonPos = ephemeris.GetMoonPositionHorizon ();
            moonPos.Normalize ();
            double moonAngle = DEGREES (Math.Asin (moonPos.y));
            
            //if (moonAngle > -18)
            {
                const double Esm = 1905.0;
                // W/m2
                const double C = 0.072;
                const double Rm = 1738.1 * 1000.0;
                // m
                double d = ephemeris.GetMoonDistanceKM () * 1000.0;
                
                // The equations for the illumination from the moon below assume that
                // the moon phase angle is 1 when full and 0 when new, which is the
                // opposite of the convention assumed by the Ephemeris class. So,
                // we assign the Earth's phase to the moon phase angle (which is always
                // its opposite) and take the opposite of that to determine the moon
                // phase angle for the purposes of these calculations.
                
                double epsilon = 0.001;
                
                double ePhase = ephemeris.GetMoonPhaseAngle ();
                if (ePhase < epsilon)
                    ePhase = epsilon;
                
                double alpha = PI - ePhase;
                while (alpha < 0) {
                    alpha += 2.0 * PI;
                }
                if (alpha < epsilon)
                    alpha = epsilon;
                
                // Earthshine:
                double Eem = 0.19 * 0.5 * (1.0 - Math.Sin (ePhase / 2.0) * Math.Tan (ePhase / 2.0) * Math.Log (1.0 / Math.Tan (ePhase / 4.0)));
                
                // Total moonlight:
                double Em = ((2.0 * C * Rm * Rm) / (3.0 * d * d)) * (Eem + Esm * (1.0 - Math.Sin (alpha / 2.0) * Math.Tan (alpha / 2.0) * Math.Log (1.0 / Math.Tan (alpha / 4.0))));
                
                double nits = NITS (Em);
                
                nits *= 0.001;
                
                if (moonAngle < 0) {
                    nits = nits * Math.Exp (1.1247 * moonAngle);
                }
                
                luminance = (float)nits;
            }
        }
        
        return luminance;
    }

    private void InitTwilightLuminances ()
    {
        twilightLuminance = new Dictionary<int, float> ();
        
        twilightLuminance[5] = 2200.0f / PIf;
        twilightLuminance[4] = 1800.0f / PIf;
        twilightLuminance[3] = 1400.0f / PIf;
        twilightLuminance[2] = 1200.0f / PIf;
        twilightLuminance[1] = 710.0f / PIf;
        twilightLuminance[0] = 400.0f / PIf;
        twilightLuminance[-1] = 190.0f / PIf;
        twilightLuminance[-2] = 77.0f / PIf;
        twilightLuminance[-3] = 28.0f / PIf;
        twilightLuminance[-4] = 9.4f / PIf;
        twilightLuminance[-5] = 2.9f / PIf;
        twilightLuminance[-6] = 0.9f / PIf;
        twilightLuminance[-7] = 0.3f / PIf;
        twilightLuminance[-8] = 0.11f / PIf;
        twilightLuminance[-9] = 0.047f / PIf;
        twilightLuminance[-10] = 0.021f / PIf;
        twilightLuminance[-11] = 0.0092f / PIf;
        twilightLuminance[-12] = 0.0031f / PIf;
        twilightLuminance[-13] = 0.0022f / PIf;
        twilightLuminance[-14] = 0.0019f / PIf;
        twilightLuminance[-15] = 0.0018f / PIf;
        twilightLuminance[-16] = 0.0018f / PIf;
    }

    public bool GetLightingChanged ()
    {
        return lightingChanged;
    }

    public Vector3 GetSunOrMoonPosition ()
    {
        if (ephemeris != null) {
            if (sunTransmittedLuminance.sqrMagnitude > moonTransmittedLuminance.sqrMagnitude) {
                return ephemeris.GetSunPositionHorizon ();
            } else {
                return ephemeris.GetMoonPositionHorizon ();
            }
        } else {
            return new Vector3 (0, 1, 0);
        }
    }

    public Color GetSunOrMoonColor ()
    {
        Vector3 sunXYZ = sunTransmittedLuminance * (float)sunLuminanceScale;
        Vector3 moonXYZ = moonTransmittedLuminance * (float)moonLuminanceScale;
        
        SilverLiningLuminanceMapper.DurandMapperXYZ (ref sunXYZ);
        SilverLiningLuminanceMapper.DurandMapperXYZ (ref moonXYZ);
        
        Vector3 XYZ = (sunXYZ + moonXYZ);
        
        Vector3 rgb = XYZ * XYZ2RGB;
        
        ApplyGamma (ref rgb);
        
        return new Color (rgb.x, rgb.y, rgb.z);
    }

    private SilverLiningEphemeris ephemeris;
    private Dictionary<int, float> twilightLuminance;

    public double T = 2.2;
    private double lastT = 0;
    private double lastSunT = 0, lastMoonT = 0;
    private double lastSunZenith = 0, lastMoonZenith = 0;
    private bool lightingChanged = true;

    private double AY, BY, CY, DY, EY;
    // Perez luminance coefficients
    private double Ax, Bx, Cx, Dx, Ex;
    // Perez chromaticity coefficients
    private double Ay, By, Cy, Dy, Ey;

    private double thetaS;
    // Angle between sun and zenith
    private double thetaM;
    // Angle between moon and zenith
    private double xZenith, yZenith, YZenith, xMoon, yMoon, YMoon;

    private double sunx, suny, sunY;
    private double moonx, moony, moonY;

    public double maxSkylightLuminance = 1.0;

    private Color skyLight;
    private Vector3 sunTransmittedLuminance, moonTransmittedLuminance;
    private Vector3 sunScatteredLuminance, moonScatteredLuminance;

    private SilverLiningSpectrum sunSpectrum, lunarSpectrum;

    private bool isOvercast = false;
    private float overcastBlend = 1.0f, overcastTransmission = 0.2f;
    public double lightPollution = 0;
    private double isothermalEffect = 1.0;

    private SilverLiningMatrix3 XYZ2RGB;
    private Matrix4x4 XYZ2RGB4;

    private GameObject sun;
    private GameObject sunLight;

    private GameObject moon;
    private GameObject moonLight;
    private Texture[] moonTextures;

    private Shader starFogShader, starNoFogShader;

    private SilverLiningStars stars;

    public float sunLightScale = 1.0f, moonLightScale = 1.0f, ambientLightScale = 1.0f;
}


