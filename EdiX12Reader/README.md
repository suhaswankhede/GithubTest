# EDI X12 Reader

A modern web-based application for parsing and viewing EDI X12 format files, specifically designed for healthcare transactions.

## Supported EDI Formats

- **837** - Healthcare Claim (Professional, Institutional, Dental)
- **835** - Healthcare Claim Payment/Remittance Advice

## Features

- ✅ Parse EDI X12 format 837 and 835 files
- ✅ View segment-by-segment breakdown
- ✅ Display element data for each segment
- ✅ Sample EDI files included for testing
- ✅ Error handling for invalid EDI content
- ✅ Clean, modern UI built with Blazor
- ✅ Real-time parsing with interactive server components

## Technology Stack

- **ASP.NET Core 8.0**
- **Blazor Server**
- **C# .NET 8.0**
- **Bootstrap 5** for responsive UI

## Getting Started

### Prerequisites

- .NET 8.0 SDK or later

### Running the Application

1. Navigate to the project directory:
   ```bash
   cd EdiX12Reader
   ```

2. Run the application:
   ```bash
   dotnet run
   ```

3. Open your browser and navigate to:
   ```
   https://localhost:5001
   ```
   or
   ```
   http://localhost:5000
   ```

## Usage

1. Navigate to the **EDI X12 Reader** page from the navigation menu
2. Either:
   - Paste your EDI X12 content directly into the text area, or
   - Click one of the sample buttons (Load Sample 837 or Load Sample 835)
3. Click the **Parse EDI** button
4. View the parsed results:
   - **Document Information**: Shows metadata like sender, receiver, control numbers, etc.
   - **Segments Detail**: Displays all segments with their elements in a structured table

## Project Structure

```
EdiX12Reader/
├── Components/
│   ├── Layout/
│   │   ├── MainLayout.razor
│   │   └── NavMenu.razor
│   ├── Pages/
│   │   ├── EdiReader.razor      # Main EDI reader page
│   │   └── Home.razor            # Home page
│   └── _Imports.razor
├── Models/
│   └── EdiDocument.cs            # EDI document model
├── Services/
│   └── EdiX12Parser.cs           # EDI parsing service
├── wwwroot/
│   └── bootstrap/                # Bootstrap CSS files
├── Program.cs                     # Application entry point
└── EdiX12Reader.csproj           # Project file
```

## EDI X12 Segment Descriptions

The parser recognizes and describes common EDI X12 segments including:

- **ISA** - Interchange Control Header
- **GS** - Functional Group Header
- **ST** - Transaction Set Header
- **BHT** - Beginning of Hierarchical Transaction
- **NM1** - Individual or Organizational Name
- **N3** - Address Information
- **N4** - Geographic Location
- **REF** - Reference Identification
- **CLM** - Claim Information
- **CLP** - Claim Level Payment Information
- **SVC** - Service Payment Information
- **DTP** - Date or Time or Period
- **AMT** - Monetary Amount
- **SE** - Transaction Set Trailer
- **GE** - Functional Group Trailer
- **IEA** - Interchange Control Trailer

## Screenshots

### Home Page
![Home Page](https://github.com/user-attachments/assets/d3c4d572-a6da-4329-820b-38f9bae1590b)

### EDI Reader Page
![EDI Reader Page](https://github.com/user-attachments/assets/b0133ab5-701e-469b-a8d6-c406c1041dc0)

### Parsed EDI 837 Healthcare Claim
![Parsed EDI 837](https://github.com/user-attachments/assets/0dc1ca60-96fb-4b81-8541-70d622f4a918)

### Parsed EDI 835 Payment/Remittance
![Parsed EDI 835](https://github.com/user-attachments/assets/339f0009-57b9-4823-99e8-f60dd2c1a694)

## License

This project is open source and available under the MIT License.
