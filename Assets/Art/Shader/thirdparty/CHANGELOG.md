# Changelog

All notable changes to the URP Double-Sided Shaders package will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

---

## [1.0.5] - 2026-06-16

### Fixed
- Removed `Shadows.hlsl` include from shadow caster pass (caused `LerpWhiteTo` undeclared identifier errors)
- Shadow pass uses self-contained `DS_ApplyShadowBias` instead
- Forward pass: include `CommonMaterial.hlsl` before `Lighting.hlsl` (required for URP shadow helpers)

## [1.0.4] - 2026-06-16

### Fixed
- Shader compile errors that placed DoubleSided shaders under **Failed to compile** in the material dropdown
- Meta pass: use `UniversalFragmentMetaLit`, `SurfaceInput.hlsl`, and correct `InitializeStandardLitSurfaceData`
- Shadow pass: use URP `ApplyShadowBias` from `Shadows.hlsl`

## [1.0.3] - 2026-06-16

### Fixed
- Restored `package.json` for one-click **Add package from disk** install
- Package ID is `com.rishiraj10.urp-doublesided-shaders` — must match the manifest entry Unity creates (do not rename manually)

### Changed
- Installation docs: use Package Manager only, never duplicate in Assets

## [1.0.2] - 2026-06-16

### Fixed
- Removed `package.json` so the shaders install as regular Assets — this stops Unity's Package Manager from registering the package twice and flooding the console with duplicate key errors

## [1.0.1] - 2026-06-16

### Fixed
- Renamed package ID from `com.m2000.urp-doublesided-shaders` to `com.rishiraj10.urp-doublesided-shaders` to resolve duplicate package registration errors in Unity
- Updated installation docs: install only once under `Packages/`, never duplicate in `Assets/` and Package Manager

### Changed
- Updated author metadata to Rishiraj (GitHub, email, portfolio)

## [1.0.0] - 2024-12-16

### Added
- **Initial Release** 🎉
- Four shader variants:
  - DoubleSided/Lite/Metallic
  - DoubleSided/Lite/Specular
  - DoubleSided/Standard/Metallic
  - DoubleSided/Standard/Specular
- Full double-sided rendering support with configurable cull mode
- Back-face normal flipping for correct lighting on both sides
- Back-face material property controls:
  - Color tint
  - Metallic/Specular multiplier
  - Smoothness multiplier
- Transparent shadow casting system with:
  - Adjustable shadow intensity
  - Dithered shadow softness (4×4 Bayer matrix)
- Detail map system (Standard variants only):
  - Detail Albedo with 3 blend modes (Multiply x2, Overlay, Add)
  - Detail Normal maps with blending
  - Detail Metallic/Specular maps
  - Detail Mask support
  - UV set selection (UV0/UV1)
  - Global blend strength control
- Parallax/height mapping (Standard variants only)
- Custom Material Inspector (DoubleSidedShaderGUI)
- Support for all URP lighting features:
  - Main light shadows (cascade/screen-space)
  - Additional lights (vertex/per-pixel)
  - Shadow softness (soft/low/medium/high)
  - Reflection probes with blending
  - Screen-space ambient occlusion (SSAO)
  - Decals (DBuffer)
  - Light cookies
  - Light layers
  - Forward+ rendering
  - Global Illumination (lightmapping)
- GPU Instancing support
- DOTS Instancing support
- VR/Single-Pass Stereo rendering support
- Mobile optimization (Lite variants)

### Technical Details
- Unity 2022.3.62f3 or newer
- Universal Render Pipeline 14.0+
- Custom shadow bias implementation for maximum compatibility
- Self-contained passes (no URP helper dependencies)
- Optimized for real-time rendering

### Documentation
- Comprehensive README with usage examples
- MIT License
- Professional package metadata

---

## Developer

**Rishiraj** - Professional Unity Shader Development
- Portfolio: https://my-portfolio-rishiraj.vercel.app
- GitHub: https://github.com/Rishiraj10
- Email: m1000.pseudocide@gmail.com

---

## Roadmap

### Planned Features
- [ ] Additional blend modes for transparency
- [ ] Subsurface scattering for vegetation
- [ ] Wind animation support
- [ ] Vertex color support
- [ ] Triplanar mapping option
- [ ] Deferred rendering support
- [ ] HDRP port

### Under Consideration
- Custom lighting models
- Toon/stylized rendering variants
- Fur/hair shader variant
- Water/liquid surface variant

---

**Made with ❤️ by Rishiraj**
