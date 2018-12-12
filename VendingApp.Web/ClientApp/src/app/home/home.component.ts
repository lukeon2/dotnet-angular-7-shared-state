import { Component, Inject } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Inventory } from '../models/inventory.model';
import { Config } from '../models/config.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
})
export class HomeComponent {

  public inventories: Inventory[] = [];

  public selectedInventory: Inventory = new Inventory();

  public coinInput?: number;
  public coinsInSlot: number = 0;

  public currency: string = "USD";
  public allCurrencies: string[] = ["USD"];
  headers = { headers: new HttpHeaders({ 'Content-Type': 'application/json' }) };

  constructor(private http: HttpClient, @Inject('BASE_URL') private baseUrl: string) {
    
  }

  ngOnInit(): void {
    this.http.get<Config>(this.baseUrl + 'api/Config/Get').subscribe(result => {
      this.inventories = result.inventories;
      this.allCurrencies = result.supportedCurrencies;
      this.currency = result.selectedCurrency;
    }, error => alert('Problem with getting config'));
  }

  insertCoin() {
    if (isNaN(this.coinInput)) {
      return;
    }

    return this.http.post<Inventory[]>(this.baseUrl + 'api/Config/UpdateCoins', JSON.stringify({ amount: this.coinInput }), this.headers)
      .subscribe(result => {
        this.coinsInSlot += this.coinInput;
        this.coinInput = null;
      }, error => { this.handleError(error); });
  }

  selectProduct(product: Inventory) {
    if (product.quantity === 0) {
      return;
    }

    if (this.coinsInSlot >= product.priceByRate) {
      this.selectedInventory = product;
      this.checkout();
    } else {
      alert("Insert coin");
    }
  }

  checkout() {
    // move to services.ts
    return this.http.post<boolean>(this.baseUrl + 'api/Inventory/Checkout', JSON.stringify(this.selectedInventory), this.headers)
      .subscribe(response => {
        this.selectedInventory.quantity--;
        let change = this.coinsInSlot - this.selectedInventory.priceByRate;
        this.coinsInSlot = 0;
        this.selectedInventory = new Inventory();
        alert("Sold. Thank you! Change: " + change);
      }, error => {
        this.handleError(error);
      });
  }

  cancelPurchase() {
    if (this.coinsInSlot > 0) {

      return this.http.get<number>(this.baseUrl + 'api/Config/CancelCoins')
        .subscribe(result => {
          this.coinsInSlot = 0;
          alert('Change returned: ' + result);
        }, error => { this.handleError(error); });
      }
    }

  setCurrency() {
    if (this.coinsInSlot > 0) {
      alert('Cannot change currency when there are coins');
      return;
    }

    // move to services.ts
    return this.http.post(this.baseUrl + 'api/Config/SetCurrency', JSON.stringify({ symbol: this.currency }), this.headers)
      .subscribe(response => {
        this.inventories = [];
        return this.http.get<Config>(this.baseUrl + 'api/Config/Get/?currency=' + this.currency).subscribe(result => {
          this.inventories = result.inventories;
          alert('Products reloaded and got new prices!');
        }, error => { this.handleError(error); });
      }, error => {
        this.handleError(error);
      });
  }

  setTwoNumberDecimal($event) {
    $event.target.value = parseFloat($event.target.value).toFixed(2);
  }
  
  handleError(error) {
    // move to error handler
    if (error !== null && error.error.Validation !== null) {
      alert(error.error.Validation[0]);
    } else {
      alert("Server side error:" + error);
    }
  }
}
