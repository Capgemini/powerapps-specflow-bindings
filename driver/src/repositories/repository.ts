export default abstract class Repository {
  protected readonly webApi: Xrm.WebApiOnline;

  constructor(webApi: Xrm.WebApiOnline) {
    this.webApi = webApi;
  }
}
