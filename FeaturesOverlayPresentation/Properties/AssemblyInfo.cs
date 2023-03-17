using System.Reflection;
using System.Resources;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Windows;

// As informações gerais sobre um assembly são controladas por
// conjunto de atributos. Altere estes valores de atributo para modificar as informações
// associada a um assembly.
[assembly: AssemblyTitle("Tutorial de Uso da Estação de Trabalho")]
[assembly: AssemblyDescription("Software criado, desenvolvido e mantido por Kevin Costa. Programado em C# utilizando .NET Framework 4.8. Sistemas com suporte: Windows 7, 10 e 11.\r\n" +
    "\r\n" +
    "    • URL do projeto: https://github.com/Kevin64/FeaturesOverlayPresentation\r\n" +
    "    • Licença: (MIT) https://github.com/Kevin64/FeaturesOverlayPresentation/blob/master/LICENCE\r\n" +
    "\r\n" +
    "Este software deve ser usado em conjunto com projeto SCPD para funcionar corretamente.\r\n" +
    "\r\n" +
    "► Sistema de Controle de Patrimônio e Docentes - SCPD\r\n" +
    "    • URL do projeto: https://github.com/Kevin64/Sistema-de-controle-de-patrimonio-e-docentes\r\n" +
    "    • Licença: (MIT) https://github.com/Kevin64/Sistema-de-controle-de-patrimonio-e-docentes/blob/main/LICENCE\r\n" +
    "\r\n" +
    "Este software e suas bibliotecas (DLLs) utilizam artes, bibliotecas Open Source e códigos avulsos de terceiros, listados abaixo. Todos os créditos vão para os seus respectivos criadores e mantenedores:\r\n" +
    "\r\n" +
    "► ini-parser\r\n" +
    "    • Copyright (c) 2008 Ricardo Amores Hernández\r\n" +
    "    • URL do projeto: https://github.com/rickyah/ini-parser \r\n" +
    "    • Licença: (MIT) https://github.com/rickyah/ini-parser/blob/master/LICENSE\r\n" +
    "\r\n" +
    "► LoadingCircle (créditos a Martin Gagne)\r\n" +
    "    • URL do projeto: https://www.codeproject.com/Articles/14841/How-to-write-a-loading-circle-animation-in-NET\r\n" +
    "    • Licença: (CPOL) https://www.codeproject.com/info/cpol10.aspx\r\n" +
    "\r\n" +
    "► NewtonsoftJson\r\n" +
    "    • Copyright (c) 2007 James Newton-King\r\n" +
    "    • URL do projeto: https://github.com/JamesNK/Newtonsoft.Json\r\n" +
    "    • Licença: (MIT) https://github.com/JamesNK/Newtonsoft.Json/blob/master/LICENSE.md\r\n" +
    "\r\n" +
    "► BCrypt.Net-Next\r\n" +
    "    • Copyright (c) 2006 Damien Miller djm@mindrot.org (jBCrypt)\r\n" +
    "    • Copyright (c) 2013 Ryan D. Emerle (.Net port)\r\n" +
    "    • Copyright (c) 2016/2021 Chris McKee (.Net-core port / patches)\r\n" +
    "    • URL do projeto: https://github.com/BcryptNet/bcrypt.net\r\n" +
    "    • Licença: (MIT) https://github.com/BcryptNet/bcrypt.net/blob/main/licence.txt\r\n")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("Coleta de Hardware e Cadastro de Patrimônio")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Definir ComVisible como false torna os tipos neste assembly invisíveis
// para componentes COM. Caso precise acessar um tipo neste assembly de
// COM, defina o atributo ComVisible como true nesse tipo.
[assembly: ComVisible(false)]

//Para começar a compilar aplicativos localizáveis, configure
//<UICulture>CultureYouAreCodingWith</UICulture> no seu arquivo .csproj
//dentro de um <Grupo de Propriedade>.  Por exemplo, se você está usando o idioma inglês
//nos seus arquivos de origem, configure o <UICulture> para en-US.  Em seguida, descomente
//o atributo NeutralResourceLanguage abaixo.  Atualize o "en-US" na
//linha abaixo para coincidir com a configuração do UICulture no arquivo do projeto.

//[assembly: NeutralResourcesLanguage("en-US", UltimateResourceFallbackLocation.Satellite)]


[assembly: ThemeInfo(
    ResourceDictionaryLocation.None, //onde os dicionários de recursos de temas específicos estão localizados
                                     //(usado se algum recurso não for encontrado na página,
                                     // ou dicionários de recursos do aplicativo)
    ResourceDictionaryLocation.SourceAssembly //onde o dicionário de recursos genéricos está localizado
                                              //(usado se algum recurso não for encontrado na página,
                                              // app, ou qualquer outro dicionário de recursos de tema específico)
)]


// As informações da versão de um assembly consistem nos quatro valores a seguir:
//
//      Versão Principal
//      Versão Secundária 
//      Número da Versão
//      Revisão
//
// É possível especificar todos os valores ou usar como padrão os Números de Build e da Revisão
// usando o "*" como mostrado abaixo:
// [assembly: AssemblyVersion("1.0.*")]
[assembly: AssemblyVersion("2.0.0.0")]
[assembly: AssemblyFileVersion("2.0.0.0")]
[assembly: NeutralResourcesLanguage("pt-BR")]
