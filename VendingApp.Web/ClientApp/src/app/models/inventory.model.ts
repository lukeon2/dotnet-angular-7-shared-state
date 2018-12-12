
export class Inventory {
  constructor() {
    this.price = 0;
    this.priceByRate = 0;
  }

  public productNr: string;
  public name: string;
  public quantity: number;
  public price: number;
  public targetCurrencyRate?: number;
  public priceByRate: number;
}
