import { environment } from '../../../environments/environment';

export class ShownResults {
    get ShownResults() {
        if (environment.showResults) {
            return true;
         } else if (!environment.showResults) {
            return false;
        } else {
            console.error(`Missing property showResults in environment`);
        }
    }
}
