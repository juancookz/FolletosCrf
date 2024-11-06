# PDF Folletos Carrefour
A C# application that downloads weekly brochure images from the Carrefour Argentina API and compiles them into a PDF for convenient offline viewing. Since Carrefour’s website doesn’t currently offer a PDF download option, this tool provides an easy way to collect and save the weekly brochure as a single PDF document.

## Features
-   **Automated JSON Download (In Testing)**: Retrieves the JSON file from Carrefour Argentina’s API, containing URLs for each brochure image. (Currently verifying URL stability for weekly updates.)
-   **Targeted Image Selection**: Filters image URLs to specifically download brochures for "Market" stores in Tucumán.
-   **Image Download and Organization**: Downloads brochure images and saves them to a designated local directory.
-   **PDF Compilation**: Combines downloaded images into a single PDF file for convenient viewing and offline access.
-   **Clear Error Handling**: Provides informative error messages to help troubleshoot issues such as network problems or invalid URLs.

## Future Improvements
-   **Configurable Image Targeting**: Add options to easily customize the city and store type filters, allowing the tool to download brochures for other regions or store categories.
-   **Automated JSON Download Stability**: Ensure the JSON download URL remains consistent for each week's brochure, or implement a method to dynamically find the correct URL.

## Prerequisites

- **.NET SDK 8.0 or later**: The application is built with .NET 8.0, so you'll need to install the .NET SDK. You can download it from the [official .NET website](https://dotnet.microsoft.com/download/dotnet/8.0).
- **PDFsharp Library**: This project uses the [PDFsharp library](http://www.pdfsharp.net/) for generating PDFs. Install it via NuGet by running:
```bash
dotnet add package PdfSharp
```
