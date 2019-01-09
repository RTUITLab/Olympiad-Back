import { environment } from '../../environments/environment';
import { UserStateService } from './user-state.service';

export class BaseHttpService {

    protected get baseUrl(): string {
        return environment.baseUrl;
    }
}
