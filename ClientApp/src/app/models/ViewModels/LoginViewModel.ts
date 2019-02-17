export class LoginViewModel {
    constructor(public Email: string, public Password: string) {}
    public static Default(): LoginViewModel {return new LoginViewModel('', ''); }
}
