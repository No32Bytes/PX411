# Contributing to URP Double-Sided Shaders

First off, thank you for considering contributing to URP Double-Sided Shaders! 🎉

## How Can I Contribute?

### Reporting Bugs

Before creating bug reports, please check the existing issues to avoid duplicates. When you create a bug report, include as many details as possible:

- **Unity Version**: Which version of Unity are you using?
- **URP Version**: Which URP package version?
- **Platform**: Windows, Mac, Linux, Mobile, Console?
- **Shader Variant**: Which shader are you using (Lite/Standard, Metallic/Specular)?
- **Steps to Reproduce**: Clear steps to reproduce the issue
- **Expected Behavior**: What you expected to happen
- **Actual Behavior**: What actually happened
- **Screenshots**: If applicable
- **Console Errors**: Any error messages from the Console

### Suggesting Features

Feature requests are welcome! Please provide:

- **Use Case**: Why is this feature needed?
- **Expected Behavior**: How should it work?
- **Alternative Solutions**: Other ways you've considered solving this
- **Additional Context**: Any other relevant information

### Pull Requests

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

#### Pull Request Guidelines

- Follow the existing code style
- Add comments for complex shader code
- Update documentation if needed
- Test on multiple platforms if possible
- Keep commits focused and atomic
- Write clear commit messages

### Code Style

#### Shader Code
- Use consistent indentation (4 spaces)
- Comment complex calculations
- Use descriptive variable names
- Group related properties together
- Add section headers with visual separators

#### C# Code (Editor Scripts)
- Follow Unity C# coding standards
- Use meaningful variable names
- Add XML documentation for public methods
- Keep methods focused and single-purpose

## Development Setup

1. Clone the repository
2. Open in Unity 2022.3.62f3 or newer
3. Ensure URP 14.0+ is installed
4. Test changes in both Lite and Standard variants
5. Verify on different platforms when possible

## Testing

Before submitting:

- [ ] Test all four shader variants
- [ ] Verify double-sided rendering works correctly
- [ ] Check both front and back face lighting
- [ ] Test transparency and alpha cutout
- [ ] Verify shadow casting
- [ ] Test with different materials
- [ ] Check for console errors or warnings
- [ ] Verify Material Inspector works properly

## Questions?

Feel free to open an issue for questions or discussions!

---

**Developer**: Rishiraj  
**Email**: m1000.pseudocide@gmail.com  
**Portfolio**: https://my-portfolio-rishiraj.vercel.app/

Thank you for contributing! 🙏
