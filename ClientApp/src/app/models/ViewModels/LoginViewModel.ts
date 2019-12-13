export class LoginViewModel {
    constructor(public Login: string, public Password: string) {}
    public static Default(): LoginViewModel {return new LoginViewModel('', ''); }
}
