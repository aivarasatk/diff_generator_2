# Diff Generator 2
Generates a mismatch report from an excel and eip file
## How to launch the app

* Clone the repo
* Open the solution (.sln) file with Visual Studio
* (If needed) Restore nuget packages
* Start (default is F5)

## About

The app scans the excel file based on the navigation (in line 2).<br>
* First part of the navigation info is the GP, KD-x-x and PP. Which tell in which columns are the following columns located:
Maker ("Gamintojas"), Code ("Kodas"), Name ("Preparato Pavadinimas"). The numbers next to KD tell in which rows are the block headers
 e.g. "Geguze 2019" and where does the actual data starts. "KD-3-5" means that on third row are block headers, and on row 5 the actual data starts.
* Second part tells the program which blocks to read, and which column is which field. (#KIEKIS1, #KIEKIS2 etc.) END keyword denotes the end of blocks to read.
<br>
Products selected for mismatch evaluation are those that meet any of the criteria:

* Any of the fields have values
* Any of the fields have shapes (mostly arrows)
* Any of the fields have colored backgrounds (excluding the default colors, in the demo files its only the slighly gray color)

<br>
The app also parses the .eip file. This process is pretty straight forward - prepare the file for serialization, serialize.
<br><br>
After scanning the required files an excel difference report is generated under Reports/ folder.

### Minimal requirements for the app su successfully generate the report
* Excel must have at least one sheet with a valid navigation line
* .eip file must contain a valid <I06> xml tag
* User must select at least one sheet to check

## Demo
Demo files are in the Demo folder. One for Excel and one .eip. Once the files and sheets are selected in the app pressing "Vykdyti" will generate the report under Report folder in current app directory. In the report you will see mismatches for May and June, as they are selected in the excel. Report highlighs in red the fields that do not match and also prints out those that are in excel but not in eip and vice versa.