export class RegisterViewModel {
    constructor(
        public Email: string,
        public Password: string,
        public FirstName: string,
        public StudentId: string,
        public RecaptchaToken: string
    ) {}
    public static get Default(): RegisterViewModel {
        return new RegisterViewModel('', '', '', '', '');
    }
}
