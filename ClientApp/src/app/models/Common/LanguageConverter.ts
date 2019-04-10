export class LanguageConverter {
    private static readonly info: LanguageInfo[] =
    [
        {
            language: 'Java',
            fileExtension: '.java',
            webName: 'java',
            fileName: 'Main.java'
        },
        {
            language: 'C#',
            fileExtension: '.cs',
            webName: 'csharp',
            fileName: 'Program.cs'
        },
        {
            language: 'Pascal ABC',
            fileExtension: '.pas',
            webName: 'pasabc',
            fileName: 'Program.pas'
        },
        {
            language: 'C',
            fileExtension: '.c',
            webName: 'c',
            fileName: 'Program.c'
        },
        {
            language: 'C++',
            fileExtension: '.cpp',
            webName: 'cpp',
            fileName: 'Progran.cpp'
        },
        {
            language: 'Python 3.6.4',
            fileExtension: '.py',
            webName: 'python',
            fileName: 'Program.py'
        }
    ];
    public static fileExtensionByPrettyName(language: string) {
        return LanguageConverter.info.find(LI => LI.language === language).fileExtension;
    }

    public static fileExtensionByWebName(language: string) {
        return LanguageConverter.info.find(LI => LI.webName === language).fileExtension;
    }

    public static languages(): string[] {
        return LanguageConverter.info.map(LI => LI.language);
    }

    public static webName(language: string) {
        return LanguageConverter.info.find(LI => LI.language === language).webName;
    }

    public static normalFromWeb(language: string) {
        if(LanguageConverter.info.find(LI => LI.webName === language)){
            return LanguageConverter.info.find(LI => LI.webName === language).language;
        } else {
            return undefined;
        }
    }

    public static fileName(language: string): string {
        return LanguageConverter.info.find(LI => LI.webName === language).fileName;
    }
}



class LanguageInfo {
    language: string;
    fileExtension: string;
    webName: string;
    fileName: string;
}
