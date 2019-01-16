import { Solution } from '../models/Solution';
import { LanguageConverter } from '../models/Common/LanguageConverter';

export class SolutionHelpers {

    public static downloadSolution(solution: Solution, program: string): void {
        const element = document.createElement('a');
        element.setAttribute('href', 'data:text/plain;charset=utf-8,' + encodeURIComponent(program));
        element.setAttribute('download', LanguageConverter.fileName(solution.Language));
        element.style.display = 'none';
        document.body.appendChild(element);
        element.click();
        document.body.removeChild(element);
    }
}
