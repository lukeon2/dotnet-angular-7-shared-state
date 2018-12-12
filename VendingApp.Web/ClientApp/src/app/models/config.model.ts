import { Inventory } from "./inventory.model";

export class Config {
  constructor() {
  }

  public inventories: Inventory[];
  public supportedCurrencies: string[];
  public selectedCurrency: string;
  public coinsInSlot: number;
}
