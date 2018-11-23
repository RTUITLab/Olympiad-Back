import { environment } from '../../environments/environment';

export class EndPoints {
    protected get baseUrl(): string {
        return environment.baseUrl;
    }
}
