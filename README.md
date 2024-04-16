# Bibliografía:
Raquel A. N. M03.UF5. Teoria. [https://learn.microsoft.com/es-es/dotnet/csharp/tour-of-csharp/tutorials/arrays-and-collections](https://docs.google.com/document/d/1JJhB0bqxqns69wKZyF-s8eacxkzv55gO-ez6QvXEvhw/edit). Published 26 de febrero de 2024.

RaquelAlamanITB. bloc-programacio/m03programacio/uf5/csvparser at main · RaquelAlamanITB/bloc-programacio. GitHub. https://github.com/RaquelAlamanITB/bloc-programacio/tree/main/m03programacio/uf5/csvparser.

RaquelAlamanITB. bloc-programacio/m03programacio/uf5/xmlparsers at main · RaquelAlamanITB/bloc-programacio. GitHub. https://github.com/RaquelAlamanITB/bloc-programacio/tree/main/m03programacio/uf5/xmlparsers.

# Webgrafía:

## Chat GPT:

- **Prompt 1**:
quiero paginar la vista de mi datagrid

- **Response**:
```
private List<Comarca> allComarcas; // Asegúrate de tener acceso a todos los datos
private int pageSize = 10; // Define el tamaño de página deseado
private int currentPage = 1; // Página actual, comienza en 1

private void PaginateData()
{
    // Calcular el índice de inicio y fin de los datos en función de la página actual y el tamaño de página
    int startIndex = (currentPage - 1) * pageSize;
    int endIndex = Math.Min(startIndex + pageSize - 1, allComarcas.Count - 1);

    // Obtener los datos correspondientes a la página actual
    List<Comarca> currentPageData = allComarcas.GetRange(startIndex, endIndex - startIndex + 1);

    // Asignar los datos al DataGridView
    dataGridView1.DataSource = currentPageData;

    // Actualizar la etiqueta de la página actual
    labelPage.Text = $"Página {currentPage}";

    // Actualizar la visibilidad de los botones de paginación según sea necesario
    buttonNext.Enabled = endIndex < allComarcas.Count - 1;
    buttonPrevious.Enabled = currentPage > 1;
}

private void buttonNext_Click(object sender, EventArgs e)
{
    currentPage++; // Incrementar la página actual
    PaginateData(); // Actualizar la vista del DataGridView
}

private void buttonPrevious_Click(object sender, EventArgs e)
{
    currentPage--; // Decrementar la página actual
    PaginateData(); // Actualizar la vista del DataGridView
}
```

- **Prompt 2**:
estoy haciendo un windows form y quiero rellenar un datagrid con los datos que leo de un CSV, como puedo hacerlo en c#?

- **Response**: 
```
        private void Form1_Load(object sender, EventArgs e)
        {
            string filePath = "ruta/de/tu/archivo.csv"; // Ruta de tu archivo CSV
            LoadCSVData(filePath);
        }

        private void LoadCSVData(string filePath)
        {
            // Verifica si el archivo existe
            if (!File.Exists(filePath))
            {
                MessageBox.Show("El archivo CSV no existe.");
                return;
            }

            // Limpia los datos existentes en el DataGridView
            dataGridView1.Rows.Clear();
            dataGridView1.Columns.Clear();

            // Lee el archivo CSV línea por línea
            using (StreamReader reader = new StreamReader(filePath))
            {
                string line;
                bool isFirstLine = true;

                while ((line = reader.ReadLine()) != null)
                {
                    // Divide la línea en campos utilizando la coma como delimitador
                    string[] fields = line.Split(',');

                    // Si es la primera línea, agrega las columnas al DataGridView
                    if (isFirstLine)
                    {
                        foreach (string field in fields)
                        {
                            dataGridView1.Columns.Add(field, field);
                        }
                        isFirstLine = false;
                    }
                    else
                    {
                        // Agrega los datos al DataGridView
                        dataGridView1.Rows.Add(fields);
                    }
                }
            }
        }
```

- **Prompt 3**:
System.NullReferenceException: 'Object reference not set to an instance of an object.'

- **Response**:
El error System.NullReferenceException: 'Object reference not set to an instance of an object.' significa que estás intentando acceder a un objeto que no ha sido inicializado (es decir, es null). En este caso, el error probablemente esté relacionado con la lista Comarques que estás intentando utilizar sin haberla inicializado previamente.

Para solucionar este error, asegúrate de inicializar la lista Comarques antes de usarla. Puedes hacerlo de la siguiente manera:
```
List<Comarca> Comarques = new List<Comarca>();
```
