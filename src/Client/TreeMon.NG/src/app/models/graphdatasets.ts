export class GraphData {
    labels: string[] = [];
    datasets: DataSet = new DataSet();
}

export class DataSet {

    data: number[] = [];
    backgroundColor: string[] = [];
    hoverBackgroundColor: string[] = [];
}
