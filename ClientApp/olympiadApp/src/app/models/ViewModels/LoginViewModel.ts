export class LoginViewModel {
    constructor(public UserName: string, public Password: string) {}
    public static Default(): LoginViewModel {return new LoginViewModel('', ''); }
}
