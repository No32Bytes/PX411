/*
 * ════════════════════════════════════════════════════════════════════════════════
 * URP Double-Sided Shaders - Material Inspector (Editor Script)
 * ════════════════════════════════════════════════════════════════════════════════
 * 
 * Version: 1.0.0
 * Author: Rishiraj
 * Website: https://my-portfolio-rishiraj.vercel.app
 * Repository: https://github.com/Rishiraj10/urp-doublesided-shaders
 * 
 * Description:
 * Custom Material Inspector for URP Double-Sided Shaders. Provides a unified,
 * production-ready editor interface with organized foldout sections for all
 * shader properties.
 * 
 * Features:
 * - Organized property sections with foldouts
 * - Surface type controls (Opaque/Cutout/Transparent)
 * - Blend mode selection for transparent materials
 * - Automatic keyword management
 * - Render queue and render type updates
 * - Professional UI layout matching Unity's standard inspectors
 * 
 * Usage:
 * This script is automatically applied to all DoubleSided shaders via the
 * CustomEditor directive in each shader file.
 * 
 * License: MIT
 * Copyright (c) 2024 Rishiraj
 * 
 * ════════════════════════════════════════════════════════════════════════════════
 */

#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

public class DoubleSidedShaderGUI : ShaderGUI
{
    // ──────────────────────────────────────────────────────────────────────────────
    // Enums
    // ──────────────────────────────────────────────────────────────────────────────
    public enum SurfaceType  { Opaque, Cutout, Transparent }
    public enum BlendMode    { Alpha, Premultiply, Additive, Multiply }
    public enum CullMode     { Front = 0, Both = 1, Back = 2 }
    public enum WorkflowMode { Metallic, Specular }
    public enum DetailBlend  { MultiplyX2, Overlay, Add }
    public enum DetailUVSet  { UV0, UV1 }

    // ──────────────────────────────────────────────────────────────────────────────
    // State
    // ──────────────────────────────────────────────────────────────────────────────
    bool _showSurface    = true;
    bool _showMain       = true;
    bool _showMaps       = true;
    bool _showEmission   = false;
    bool _showDetail     = false;
    bool _showDoubleSide = true;
    bool _showShadow     = false;
    bool _showAdvanced   = false;

    // ──────────────────────────────────────────────────────────────────────────────
    // OnGUI
    // ──────────────────────────────────────────────────────────────────────────────
    public override void OnGUI(MaterialEditor editor, MaterialProperty[] props)
    {
        Material mat = editor.target as Material;
        bool isStandard = mat.shader.name.Contains("Standard");
        bool isSpecular = mat.shader.name.Contains("Specular");

        EditorGUI.BeginChangeCheck();

        // ── Surface Type ──────────────────────────────────────────────────────────
        _showSurface = DrawFoldout("Surface", _showSurface);
        if (_showSurface)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                SurfaceType surfType = GetSurfaceType(mat);
                surfType = (SurfaceType)EditorGUILayout.EnumPopup("Surface Type", surfType);
                SetSurfaceType(mat, surfType);

                if (surfType == SurfaceType.Transparent)
                {
                    BlendMode blendMode = GetBlendMode(mat);
                    blendMode = (BlendMode)EditorGUILayout.EnumPopup("Blend Mode", blendMode);
                    SetBlendMode(mat, blendMode);
                }

                MaterialProperty cutoff = FindProperty("_Cutoff", props, false);
                if (surfType == SurfaceType.Cutout && cutoff != null)
                    editor.ShaderProperty(cutoff, "Alpha Cutoff");
            }
        }

        EditorGUILayout.Space(4);

        // ── Main Maps ─────────────────────────────────────────────────────────────
        _showMain = DrawFoldout("Main Maps", _showMain);
        if (_showMain)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                DrawProp(editor, props, "_BaseMap",   "Albedo");
                DrawProp(editor, props, "_BaseColor", "Color");

                EditorGUILayout.Space(2);

                if (!isSpecular)
                {
                    // Metallic workflow
                    DrawProp(editor, props, "_MetallicGlossMap", "Metallic (R) Smoothness (A)");
                    MaterialProperty metProp = FindProperty("_MetallicGlossMap", props, false);
                    bool hasMetMap = metProp != null && metProp.textureValue != null;
                    if (!hasMetMap) DrawProp(editor, props, "_Metallic",   "Metallic");
                    DrawProp(editor, props, "_Glossiness",   "Smoothness");
                    DrawProp(editor, props, "_GlossMapScale","Smoothness Scale");
                    SetKeyword(mat, "_METALLICSPECGLOSSMAP", hasMetMap);
                }
                else
                {
                    // Specular workflow
                    DrawProp(editor, props, "_SpecGlossMap", "Specular (RGB) Smoothness (A)");
                    MaterialProperty specProp = FindProperty("_SpecGlossMap", props, false);
                    bool hasSpecMap = specProp != null && specProp.textureValue != null;
                    if (!hasSpecMap) DrawProp(editor, props, "_SpecColor", "Specular Color");
                    DrawProp(editor, props, "_Glossiness",   "Smoothness");
                    DrawProp(editor, props, "_GlossMapScale","Smoothness Scale");
                    SetKeyword(mat, "_SPECGLOSSMAP", hasSpecMap);
                }
            }
        }

        EditorGUILayout.Space(4);

        // ── Surface Maps ──────────────────────────────────────────────────────────
        _showMaps = DrawFoldout("Surface Maps", _showMaps);
        if (_showMaps)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                DrawProp(editor, props, "_BumpMap",  "Normal Map");
                MaterialProperty bumpProp = FindProperty("_BumpMap", props, false);
                bool hasNormal = bumpProp != null && bumpProp.textureValue != null;
                if (hasNormal) DrawProp(editor, props, "_BumpScale", "Normal Scale");
                SetKeyword(mat, "_NORMALMAP", hasNormal);

                EditorGUILayout.Space(2);

                DrawProp(editor, props, "_OcclusionMap", "Occlusion");
                MaterialProperty occProp = FindProperty("_OcclusionMap", props, false);
                bool hasOcc = occProp != null && occProp.textureValue != null;
                if (hasOcc) DrawProp(editor, props, "_OcclusionStrength", "Occlusion Strength");
                SetKeyword(mat, "_OCCLUSIONMAP", hasOcc);

                // Parallax (Standard only)
                if (isStandard)
                {
                    EditorGUILayout.Space(2);
                    MaterialProperty parallaxEnabled = FindProperty("_ParallaxEnabled", props, false);
                    bool useParallax = parallaxEnabled != null && parallaxEnabled.floatValue > 0.5f;
                    useParallax = EditorGUILayout.Toggle("Parallax Mapping", useParallax);
                    if (parallaxEnabled != null) parallaxEnabled.floatValue = useParallax ? 1f : 0f;
                    if (useParallax)
                    {
                        DrawProp(editor, props, "_ParallaxMap", "Height Map");
                        DrawProp(editor, props, "_Parallax",    "Parallax Scale");
                    }
                    SetKeyword(mat, "_PARALLAXMAP", useParallax);
                }
            }
        }

        EditorGUILayout.Space(4);

        // ── Emission ──────────────────────────────────────────────────────────────
        _showEmission = DrawFoldout("Emission", _showEmission);
        if (_showEmission)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                MaterialProperty emEnabled = FindProperty("_EmissionEnabled", props, false);
                bool emOn = emEnabled != null && emEnabled.floatValue > 0.5f;
                emOn = EditorGUILayout.Toggle("Enable Emission", emOn);
                if (emEnabled != null) emEnabled.floatValue = emOn ? 1f : 0f;
                SetKeyword(mat, "_EMISSION", emOn);
                if (emOn)
                {
                    DrawProp(editor, props, "_EmissionMap",   "Emission Map");
                    DrawProp(editor, props, "_EmissionColor", "Emission Color");
                }
            }
        }

        EditorGUILayout.Space(4);

        // ── Detail Maps (Standard only) ───────────────────────────────────────────
        if (isStandard)
        {
            _showDetail = DrawFoldout("Detail Maps", _showDetail);
            if (_showDetail)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    MaterialProperty detEnabled = FindProperty("_DetailEnabled", props, false);
                    bool detOn = detEnabled != null && detEnabled.floatValue > 0.5f;
                    detOn = EditorGUILayout.Toggle("Enable Detail Maps", detOn);
                    if (detEnabled != null) detEnabled.floatValue = detOn ? 1f : 0f;
                    SetKeyword(mat, "_DETAIL_MULX2", detOn);

                    if (detOn)
                    {
                        DrawProp(editor, props, "_UVSec",           "UV Set");
                        DrawProp(editor, props, "_DetailMask",      "Detail Mask (A channel)");

                        EditorGUILayout.Space(2);
                        EditorGUILayout.LabelField("Albedo", EditorStyles.boldLabel);
                        DrawProp(editor, props, "_DetailAlbedoMap",      "Detail Albedo");
                        DrawProp(editor, props, "_DetailAlbedoStrength", "Albedo Strength");

                        EditorGUILayout.Space(2);
                        EditorGUILayout.LabelField("Normal", EditorStyles.boldLabel);
                        DrawProp(editor, props, "_DetailNormalMap",      "Detail Normal Map");
                        DrawProp(editor, props, "_DetailNormalMapScale", "Normal Scale");

                        EditorGUILayout.Space(2);
                        if (!isSpecular)
                        {
                            EditorGUILayout.LabelField("Metallic / Smoothness", EditorStyles.boldLabel);
                            DrawProp(editor, props, "_DetailMetallicMap",       "Detail Metallic Map");
                            DrawProp(editor, props, "_DetailMetallicStrength",  "Metallic Blend");
                        }
                        else
                        {
                            EditorGUILayout.LabelField("Specular / Smoothness", EditorStyles.boldLabel);
                            DrawProp(editor, props, "_DetailSpecMap",     "Detail Specular Map");
                            DrawProp(editor, props, "_DetailSpecStrength","Specular Blend");
                        }
                        DrawProp(editor, props, "_DetailSmoothnessStrength", "Smoothness Blend");

                        EditorGUILayout.Space(2);
                        DrawProp(editor, props, "_DetailBlendMode",     "Blend Mode");
                        DrawProp(editor, props, "_DetailBlendStrength", "Global Blend Strength");
                    }
                }
            }
            EditorGUILayout.Space(4);
        }

        // ── Double-Sided ──────────────────────────────────────────────────────────
        _showDoubleSide = DrawFoldout("Double-Sided", _showDoubleSide);
        if (_showDoubleSide)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                DrawProp(editor, props, "_CullMode",          "Cull Mode");
                DrawProp(editor, props, "_BackFaceNormalFlip","Flip Back Face Normals");
                DrawProp(editor, props, "_BackFaceColor",     "Back Face Tint");

                if (isStandard)
                {
                    if (!isSpecular)
                    {
                        DrawProp(editor, props, "_BackFaceMetallicMult",   "Back Face Metallic Multiplier");
                    }
                    else
                    {
                        DrawProp(editor, props, "_BackFaceSpecMult",       "Back Face Specular Multiplier");
                    }
                    DrawProp(editor, props, "_BackFaceSmoothnessMult", "Back Face Smoothness Multiplier");
                }
            }
        }

        EditorGUILayout.Space(4);

        // ── Transparent Shadows ───────────────────────────────────────────────────
        SurfaceType currentSurface = GetSurfaceType(mat);
        if (currentSurface == SurfaceType.Transparent || currentSurface == SurfaceType.Cutout)
        {
            _showShadow = DrawFoldout("Shadow Settings", _showShadow);
            if (_showShadow)
            {
                using (new EditorGUI.IndentLevelScope())
                {
                    DrawProp(editor, props, "_ShadowIntensity",  "Shadow Intensity");
                    DrawProp(editor, props, "_ShadowDitherScale","Dither Softness");
                }
            }
            EditorGUILayout.Space(4);
        }

        // ── Advanced ──────────────────────────────────────────────────────────────
        _showAdvanced = DrawFoldout("Advanced", _showAdvanced);
        if (_showAdvanced)
        {
            using (new EditorGUI.IndentLevelScope())
            {
                editor.EnableInstancingField();
                editor.DoubleSidedGIField();
                editor.RenderQueueField();
            }
        }

        if (EditorGUI.EndChangeCheck())
            foreach (Material m in editor.targets)
                ApplyMaterialKeywords(m);
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // Surface Type Logic
    // ──────────────────────────────────────────────────────────────────────────────
    SurfaceType GetSurfaceType(Material mat)
    {
        float v = mat.GetFloat("_SurfaceType");
        return (SurfaceType)Mathf.RoundToInt(v);
    }

    BlendMode GetBlendMode(Material mat)
    {
        float v = mat.GetFloat("_BlendMode");
        return (BlendMode)Mathf.RoundToInt(v);
    }

    void SetSurfaceType(Material mat, SurfaceType type)
    {
        mat.SetFloat("_SurfaceType", (float)type);

        SetKeyword(mat, "_ALPHATEST_ON",       type == SurfaceType.Cutout);
        SetKeyword(mat, "_ALPHAPREMULTIPLY_ON", false);
        SetKeyword(mat, "_SURFACE_TYPE_TRANSPARENT", type == SurfaceType.Transparent);

        switch (type)
        {
            case SurfaceType.Opaque:
                mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetFloat("_ZWrite",   1f);
                mat.renderQueue = (int)RenderQueue.Geometry;
                mat.SetOverrideTag("RenderType", "Opaque");
                break;

            case SurfaceType.Cutout:
                mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                mat.SetFloat("_ZWrite",   1f);
                mat.renderQueue = (int)RenderQueue.AlphaTest;
                mat.SetOverrideTag("RenderType", "TransparentCutout");
                break;

            case SurfaceType.Transparent:
                SetBlendMode(mat, GetBlendMode(mat));
                mat.SetFloat("_ZWrite",   0f);
                mat.renderQueue = (int)RenderQueue.Transparent;
                mat.SetOverrideTag("RenderType", "Transparent");
                break;
        }
    }

    void SetBlendMode(Material mat, BlendMode mode)
    {
        mat.SetFloat("_BlendMode", (float)mode);
        switch (mode)
        {
            case BlendMode.Alpha:
                mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                SetKeyword(mat, "_ALPHAPREMULTIPLY_ON", false);
                break;
            case BlendMode.Premultiply:
                mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.One);
                mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
                SetKeyword(mat, "_ALPHAPREMULTIPLY_ON", true);
                break;
            case BlendMode.Additive:
                mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.SrcAlpha);
                mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.One);
                SetKeyword(mat, "_ALPHAPREMULTIPLY_ON", false);
                break;
            case BlendMode.Multiply:
                mat.SetFloat("_SrcBlend", (float)UnityEngine.Rendering.BlendMode.DstColor);
                mat.SetFloat("_DstBlend", (float)UnityEngine.Rendering.BlendMode.Zero);
                SetKeyword(mat, "_ALPHAPREMULTIPLY_ON", false);
                break;
        }
    }

    void ApplyMaterialKeywords(Material mat)
    {
        SetSurfaceType(mat, GetSurfaceType(mat));
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // Helpers
    // ──────────────────────────────────────────────────────────────────────────────
    static void SetKeyword(Material mat, string keyword, bool enabled)
    {
        if (enabled) mat.EnableKeyword(keyword);
        else         mat.DisableKeyword(keyword);
    }

    static void DrawProp(MaterialEditor editor, MaterialProperty[] props, string name, string label = null)
    {
        MaterialProperty p = FindProperty(name, props, false);
        if (p != null) editor.ShaderProperty(p, label ?? p.displayName);
    }

    static bool DrawFoldout(string label, bool state)
    {
        GUIStyle style = new GUIStyle(EditorStyles.foldout) { fontStyle = FontStyle.Bold };
        return EditorGUILayout.Foldout(state, label, true, style);
    }

    // ──────────────────────────────────────────────────────────────────────────────
    // Assign defaults on first material creation
    // ──────────────────────────────────────────────────────────────────────────────
    public override void AssignNewShaderToMaterial(Material mat, Shader oldShader, Shader newShader)
    {
        base.AssignNewShaderToMaterial(mat, oldShader, newShader);
        SetSurfaceType(mat, SurfaceType.Opaque);
    }
}
#endif
