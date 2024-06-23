## This folder contains the necessary files and resources for the `KeyPhrase Analysis` project.

## Description
The codebase is provided as a "POC" to make your GPT engine behave as a `RAG` model for you in which you give it the 
important keywords from the paper of an engineering entrance exam question paper and tell it to tell you the important topics to prepare
for the exma based on the set of keywords given to it.

## Installation
To install the required packages, run the following commands in the terminal:

1) `dotnet add package Azure.AI.OpenAI --version 1.0.0-beta.14`
2) `dotnet add package Microsoft.Extensions.Configuration --version 8.0.0`
3) `dotnet add package Microsoft.Extensions.Configuration.Json`
4) `dotnet add package Azure.Search.Documents`
5) `dotnet add package Newtonsoft.Json`

## Usage
run the following script:
`dotnet build`
`dotnet run`

## Contributing
Contributions are welcome! If you have any suggestions or improvements, feel free to open an issue or submit a pull request.

