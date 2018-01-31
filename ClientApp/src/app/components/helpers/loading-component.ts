export class LoadingComponent {
    private loadingVar = false;
    public get loading() { return this.loadingVar; }

    constructor(initLoading: boolean = false) {
        this.loadingVar = initLoading;
    }
    protected startLoading() { this.loadingVar = true; }
    protected stopLoading() { this.loadingVar = false; }
}
