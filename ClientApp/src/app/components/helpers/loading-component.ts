export class LoadingComponent {
    constructor(public loading: boolean = false) {}
    protected startLoading() { this.loading = true; }
    protected stopLoading() { this.loading = false; }
}
