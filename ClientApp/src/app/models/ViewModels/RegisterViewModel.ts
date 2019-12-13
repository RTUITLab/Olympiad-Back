export class RegisterViewModel {
    constructor(
        public FirstName: string,
        public LastName: string,
        public Email: string,
        public Password: string,
        public StudentId: string,
        public RecaptchaToken: string
    ) {}
    public static get Default(): RegisterViewModel {
        return new RegisterViewModel( '', '', '', '', '', '');
    }
}
