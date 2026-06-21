# URP Double-Sided Shaders

**By Rishiraj** | Professional Unity Shader Development

[![Unity Version](https://img.shields.io/badge/Unity-2022.3.62f3+-blue.svg)](https://unity.com/)
[![URP](https://img.shields.io/badge/URP-14.0+-green.svg)](https://docs.unity3d.com/Packages/com.unity.render-pipelines.universal@latest)
[![License](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE.md)

Enhanced URP shaders with full double-sided rendering support — production-ready drop-in alternatives to Unity's default shaders.

---

## 🎯 Overview

This package extends Unity's standard URP shaders by adding full double-sided rendering support, while keeping a familiar, optimized, and production-ready workflow. Designed as enhanced alternatives to Unity's default shaders, these shaders provide more flexibility, additional controls, and extended rendering options while remaining lightweight and efficient for real-time projects.

**Perfect for:** Foliage, fabric, thin surfaces, environment assets, stylized props, and any material requiring visible front and back faces without duplicated geometry.

---

## Package Structure

```
URP_DoubleSided/
├── package.json                      ← Package Manager manifest
├── Editor/
│   └── DoubleSidedShaderGUI.cs       ← Custom Material Inspector
└── Shaders/
    ├── Lite/
    │   ├── DoubleSided_Lite_Metallic.shader
    │   └── DoubleSided_Lite_Specular.shader
    └── Standard/
        ├── DoubleSided_Standard_Metallic.shader
        └── DoubleSided_Standard_Specular.shader
```

---

## Installation (one click)

1. Open Unity → **Window → Package Manager**
2. Click **+** (top-left) → **Add package from disk…**
3. Select `package.json` inside this `URP_DoubleSided` folder
4. Done — shaders appear under **DoubleSided/** in the shader dropdown

> **Use only this method.** Do not also copy the folder into `Assets/` or `Packages/` manually. Installing twice causes the duplicate key error.

### If you already see duplicate key errors

1. **Close Unity**
2. Delete `Assets/URP_DoubleSided` if it exists
3. Delete `Packages/com.rishiraj10.urp-doublesided-shaders` if it exists
4. Open `Packages/manifest.json` and remove any line with `urp-doublesided-shaders`
5. Reopen Unity → use **Add package from disk** once (step above)

### Requirements
- Unity **2022.3.62f3** or newer
- Universal Render Pipeline (**URP 14.0+**)
- Forward Rendering path

---

## Shader Variants

| Shader | Workflow | Detail Maps | Parallax |
|--------|----------|-------------|---------|
| `DoubleSided/Lite/Metallic`    | Metallic | ✗ | ✗ |
| `DoubleSided/Lite/Specular`    | Specular | ✗ | ✗ |
| `DoubleSided/Standard/Metallic`| Metallic | ✓ | ✓ |
| `DoubleSided/Standard/Specular`| Specular | ✓ | ✓ |

---

## Material Inspector

All shaders share a unified custom Material Inspector with foldout sections:

### Surface
| Property | Description |
|----------|-------------|
| **Surface Type** | Opaque / Cutout / Transparent |
| **Blend Mode** | Alpha / Premultiply / Additive / Multiply *(Transparent only)* |
| **Alpha Cutoff** | Clip threshold *(Cutout only)* |

### Main Maps
| Property | Description |
|----------|-------------|
| **Albedo** | Base color texture + tint |
| **Metallic (R) Smoothness (A)** | Metallic workflow packed map |
| **Specular (RGB) Smoothness (A)** | Specular workflow map |
| **Smoothness** | Surface roughness (0 = rough, 1 = mirror) |
| **Smoothness Scale** | Multiplier when using a packed map |

### Surface Maps
| Property | Description |
|----------|-------------|
| **Normal Map** | Tangent-space normal map |
| **Normal Scale** | Normal map intensity |
| **Occlusion** | Ambient occlusion map (G channel) |
| **Occlusion Strength** | AO blend amount |
| **Parallax Mapping** | *(Standard only)* Depth offset using height map |

### Detail Maps *(Standard only)*
| Property | Description |
|----------|-------------|
| **UV Set** | UV0 or UV1 |
| **Detail Mask** | Alpha channel masks detail influence |
| **Detail Albedo** | Secondary albedo (blended on top of base) |
| **Detail Normal** | Secondary normal map |
| **Detail Metallic / Specular** | Secondary PBR map |
| **Blend Mode** | Multiply x2, Overlay, or Add |
| **Global Blend Strength** | Master control for all detail blending |

### Double-Sided
| Property | Description |
|----------|-------------|
| **Cull Mode** | Front / **Both** (default) / Back |
| **Flip Back Face Normals** | Mirrors normals on backfaces for correct lighting |
| **Back Face Tint** | Optional per-face color tint |
| **Back Face Metallic/Specular Multiplier** | *(Standard only)* |
| **Back Face Smoothness Multiplier** | *(Standard only)* |

### Shadow Settings *(Cutout & Transparent)*
| Property | Description |
|----------|-------------|
| **Shadow Intensity** | How dark the transparent shadow appears |
| **Dither Softness** | Screen-space dither scale for soft shadow edges |

---

## Transparent Shadow System

Transparent and Cutout surfaces use **4×4 ordered dithering (Bayer matrix)** in the Shadow Caster pass to produce soft, adjustable shadow casting:

- **Shadow Intensity 1.0** = full opaque shadow
- **Shadow Intensity 0.0** = no shadow
- **Dither Softness** scales the screen-space sample pattern — increase for softer transitions

This works without any URP custom render features or post-processing.

---

## Common Use Cases

**Foliage / Vegetation**
→ `Lite/Metallic`, Surface: Cutout, Cull: Both, flip normals ON

**Cloth / Fabric**
→ `Standard/Specular`, Surface: Opaque, custom specular color, detail normal map for weave texture

**Thin meshes (cards, paper)**
→ `Lite/Specular`, Surface: Transparent, Shadow Intensity ~0.6

**Stylized props**
→ `Standard/Metallic`, detail albedo with Overlay blend for surface wear

**Environment rocks / ground**
→ `Standard/Metallic`, Parallax enabled, detail maps for micro-surface

---

## Performance Notes

- **Lite shaders** compile to roughly the same number of shader variants as Unity's built-in URP Lit. Suitable for mobile.
- **Standard shaders** add detail map sampling (2–4 additional texture fetches per fragment). Suitable for PC / console.
- Double-sided rendering adds **no geometry overhead** — it only changes the GPU rasterizer cull state.
- All passes (DepthOnly, DepthNormals, Shadow, Meta) respect the cull mode so GI baking and SSAO work correctly.

---

## Compatibility

| Feature | Status |
|---------|--------|
| URP Forward | ✅ |
| GPU Instancing | ✅ |
| Lightmapping (GPU + CPU) | ✅ |
| SSAO | ✅ |
| Decals (DBuffer) | ✅ Metallic variant |
| Reflection Probe Blending | ✅ |
| Forward+ | ✅ |
| DOTS Instancing | ✅ |
| VR / Single-Pass Stereo | ✅ |
| Deferred Rendering | ⚠ Not tested |
| HDRP | ❌ Not supported |

---

## Changelog

### 1.0.0
- Initial release
- Lite Metallic, Lite Specular, Standard Metallic, Standard Specular
- Custom unified Material Inspector
- Dithered transparent shadow casting
- Back-face color tint, normal flip, PBR multipliers
- Detail map system with 3 blend modes (Standard variants)
- Parallax offset (Standard variants)

---

## 📄 License

This package is released under the MIT License. See [LICENSE.md](LICENSE.md) for details.

## 👨‍💻 Developer

**Rishiraj** - Professional Unity Shader & Graphics Development

- Portfolio: [my-portfolio-rishiraj.vercel.app](https://my-portfolio-rishiraj.vercel.app)
- GitHub: [@Rishiraj10](https://github.com/Rishiraj10)
- Email: m1000.pseudocide@gmail.com

## 🙏 Support

If you find this package useful, please consider:
- ⭐ Starring the repository
- 🐛 Reporting issues or bugs
- 💡 Suggesting new features
- 📖 Contributing to documentation
- 🔗 Sharing with other developers

## 🔗 Links

- [Documentation](https://github.com/Rishiraj10/urp-doublesided-shaders/wiki)
- [Issue Tracker](https://github.com/Rishiraj10/urp-doublesided-shaders/issues)
- [Discussions](https://github.com/Rishiraj10/urp-doublesided-shaders/discussions)

---

**Made with ❤️ by Rishiraj** | *Professional shader solutions for Unity developers*
