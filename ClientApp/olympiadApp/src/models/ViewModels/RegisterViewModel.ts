export class RegisterViewModel {
    constructor(
        public Email: string,
        public Password: string,
        public FirstName: string,
        public StudentId: string
    ) {}
    public static Default(): RegisterViewModel {
        return new RegisterViewModel('', '', '', '');
    }
}
