namespace SB.SDM.Pipeline.Pipe

open System.Windows.Forms
open System.Windows.Forms.DataVisualization.Charting

module Chart =

    type LineChartForm( title, xs : float seq ) =
        inherit Form( Text=title )

        let chart = new Chart(Dock=DockStyle.Fill)
        let area = new ChartArea(Name="Area1")
        let series = new Series()
        
        do series.ChartType <- SeriesChartType.Line
        do xs |> Seq.iter (series.Points.Add >> ignore)
        do series.ChartArea <- "Area1"
        do chart.Series.Add( series )
        do chart.ChartAreas.Add(area)
        do base.Controls.Add( chart )

    
    let main() =
        let data = seq { for i in 1..1000 do yield sin(float i / 100.0) }
        let f = new LineChartForm( "Sine", data )
        Application.Run(f)

