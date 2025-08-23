// Generated TypeScript client stub for TOSS ERP API
export interface ApiConfiguration {
  basePath?: string;
  accessToken?: string;
}

export class Configuration {
  constructor(private config: ApiConfiguration = {}) {}
  
  get basePath(): string {
    return this.config.basePath || 'https://api.toss-erp.com';
  }
  
  get accessToken(): string | undefined {
    return this.config.accessToken;
  }
}

export class BaseApi {
  constructor(protected configuration: Configuration) {}
}

export class DefaultApi extends BaseApi {
  async healthCheck(): Promise<{ status: string }> {
    const response = await fetch(${this.configuration.basePath}/health);
    return response.json();
  }
}
