// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.

import { Injectable } from '@angular/core';
import { Http, Headers, RequestOptions } from '@angular/http';
import 'rxjs/add/operator/map';
import { WebApiService } from '../services/webApi.service';
import { SessionService } from '../services/session.service';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';
import { UnitOfMeasure } from '../models/unitofmeasure';
import { Currency } from '../models/currency';
import { FinanceAccount } from '../models/financeaccount';
import { FinanceAccountTransaction } from '../models/financeaccountransaction';
import { PriceRule } from '../models/pricerule';
import { BasicValidators } from '../common/basicValidators';

@Injectable()
export class FinanceService extends WebApiService {

    constructor(http: Http, sessionService: SessionService) {
        super(http, sessionService);
    }
    getPriceRule(priceRuleCode: string) {
        return this.invokeRequest('GET', 'api/Finance/PriceRules/' + priceRuleCode);
    }
    getPriceRules(filters?: Filter[]) {
        return this.invokeRequest('GET', 'api/Finance/PriceRules/' + '?filter=' + JSON.stringify(filters));
    }


    deletePriceRule(priceRuleUUID: string) {
        return this.invokeRequest('DELETE', 'api/Finance/PriceRules/Delete/' + priceRuleUUID);
    }

    addPriceRule(priceRule: PriceRule) {
        return this.invokeRequest('POST', 'api/Finance/PriceRules/Insert', JSON.stringify(priceRule));
    }

    updatePriceRule(priceRule: PriceRule) {
        return this.invokeRequest('PATCH', 'api/Finance/PriceRules/Update', JSON.stringify(priceRule));
    }

    calcPriceRule(amount: number, operator: string, operand: number): number {

        let res = 0;

        if (BasicValidators.isNullOrEmpty(operator)) { return res; }

        if (!operand) { return res; }

        switch (operator) {
            case '*':
                res = amount * operand;
                break;
            case '%':
                if (operand === 0) {
                    res = 0;
                } else {
                    const pct = operand / 100;
                    const pctChange = amount * pct;
                    res = amount + pctChange;
                }
                break;
            case '=':
                res = amount;
                break;
            case '+':
                res = Number(amount) + Number(operand);
                break;
            case '-':
                res = amount - operand;
                break;
            case '/':
                if (amount === 0 || operand === 0) {
                    console.log('Cannot divide by zero!');
                    res = 0;
                } else {
                    res = amount / operand;
                }
                break;
            default:
                console.log('FORGOT THE OPERATOR!');
                break;
        }
        return res;
    }

    getFinanceAccountTransactions(filter?: Filter) {
        return this.invokeRequest('GET', 'api/Finance/Accounts/Transactions/' + '?filter=' + JSON.stringify(filter) );
    }

    deleteFinanceAccountTransaction(financeAccountUUID: string) {
        return this.invokeRequest('DELETE', 'api/Finance/Accounts/Transactions/Delete/' + financeAccountUUID);
    }

    addFinanceAccountTransaction(financeAccount: FinanceAccountTransaction) {
        return this.invokeRequest('POST', 'api/Finance/Accounts/Transactions/Add', JSON.stringify(financeAccount));
    }

    updateFinanceAccountTransaction(currency: FinanceAccountTransaction) {
        return this.invokeRequest('PATCH', 'api/Finance/Accounts/Transactions/Update', JSON.stringify(currency));
    }

    getFinanceAccounts(filter?: Filter) {
        return this.invokeRequest('GET', 'api/Finance/Accounts/' + '?filter=' + JSON.stringify(filter));
    }

    getPaymenOptions() {
        return this.invokeRequest('GET', 'api/Finance/PaymentOptions');
    }

    deleteFinanceAccount(financeAccountUUID: string) {
        return this.invokeRequest('DELETE', 'api/Finance/Accounts/Delete/' + financeAccountUUID);
    }

    addFinanceAccount(financeAccount: FinanceAccount) {
        return this.invokeRequest('POST', 'api/Finance/Accounts/Add', JSON.stringify(financeAccount));
    }

    updateFinanceAccount(account: FinanceAccount) {
        return this.invokeRequest('PATCH', 'api/Finance/Accounts/Update', JSON.stringify(account));
    }

    getCurrencies(filter?: Filter) {
        return this.invokeRequest('GET', 'api/Finance/Currency' + '?filter=' + JSON.stringify(filter));
    }

    getCurrency(name: string) {
        return this.invokeRequest('GET', 'api/Finance/Currency/' + name);
    }

    getCurrencySymbols(filter?: Filter ) {
        return this.invokeRequest('GET', 'api/Finance/Currency/Symbols');
    }
    getAssetClasses(filter?: Filter) {
        return this.invokeRequest('GET', 'api/Finance/AssetClasses');
    }


    addCurrency(currency: Currency) {
        return this.invokeRequest('POST', 'api/Finance/Currency/Add', JSON.stringify(currency));
    }

    updateCurrency(currency: Currency) {
        return this.invokeRequest('PATCH', 'api/Finance/Currency/Update', JSON.stringify(currency));
    }

    deleteCurrency(currencyUUID: string) {
        return this.invokeRequest('DELETE', 'api/Finance/Currency/Delete/' + currencyUUID );
    }
}
