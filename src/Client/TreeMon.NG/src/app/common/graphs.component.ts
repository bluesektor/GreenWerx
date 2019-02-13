// Copyright 2015, 2017 TreeMon.org.
// Licensed under CPAL 1.0,  See license.txt  or go to http://treemon.org/docs/license.txt  for full license details.
import { ChartModule } from 'primeng/primeng';
import { Filter } from '../models/filter';
import { Screen } from '../models/screen';
import { DataPoint } from '../models/datapoint';
import { GraphData } from '../models/graphdatasets';
import { List } from 'linqts';
import { Color } from './color.utility';
import { Component, ViewChild} from '@angular/core';



@Component({
    selector: 'app-graphs',
    templateUrl: './graphs.component.html'
})


export class GraphsComponent  {

    width = 600;
    height = 400;
    type = 'column2d';
    dataFormat = 'json';
    data: any;

    @ViewChild('pieChart')   pieChart;

    constructor() {
        this.data = {
            labels: ['', '', '', '', '', '', '', '', '', ''],
            datasets: [
                {
                    data: [0, 0, 0, 0, 0, 0, 0, 0, 0, 0],
                    backgroundColor: [
                        '#000000',
                        '#000000',
                        '#000000',
                        '#000000',
                        '#000000',
                        '#000000',
                        '#000000',
                        '#000000',
                        '#000000',
                        '#000000'
                    ],
                    hoverBackgroundColor: [
                        '#000000',
                        '#000000',
                        '#000000',
                        '#000000',
                        '#000000',
                        '#000000',
                        '#000000',
                        '#000000',
                        '#000000',
                        '#000000'
                    ]
                }]
            };
    }

    // https://www.primefaces.org/primeng/#/chart

    graph( graphSet: GraphData   ) {

        let max = 10;
        if (graphSet.datasets.data.length < max) {
            max = graphSet.datasets.data.length;
        }

        for (let i = 0; i < max; i++) {
             this.data.labels[i] = graphSet.labels[i];
             this.data.datasets[0].data[i] = graphSet.datasets.data[i];
        }

        const dataMinima =   Math.min(...this.data.datasets[0].data );
        const dataMaxima = Math.max(...this.data.datasets[0].data );

        this.data.datasets[0].backgroundColor = new Array();
        this.data.datasets[0].hoverBackgroundColor = new Array();

        this.data.datasets[0].data.forEach(element => {
             this.data.datasets[0].backgroundColor.push( Color.GetRgb(dataMinima, dataMaxima, element));
             this.data.datasets[0].hoverBackgroundColor.push( Color.GetRgb(dataMinima, dataMaxima, element));
        });

       this.pieChart.refresh();
    }

}
