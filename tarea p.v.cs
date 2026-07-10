using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace MinimercadoVisual
{
    // Plantilla de Producto
    public class Producto
    {
        public string ID { get; set; }
        public string Nombre { get; set; }
        public double Precio { get; set; }
        public int Stock { get; set; }
        public Producto(string id, string nombre, double precio, int stock)
        {
            ID = id; Nombre = nombre; Precio = precio; Stock = stock;
        }
    }

    // Plantilla de Ítem en el Carrito
    public class ItemCarrito
    {
        public Producto Producto { get; set; }
        public int Cantidad { get; set; }
        public ItemCarrito(Producto producto, int cantidad)
        {
            Producto = producto; Cantidad = cantidad;
        }
        public double Subtotal => Producto.Precio * Cantidad;
    }

    public partial class Form1 : Form
    {
        // Inventario del minimercado
        private List<Producto> inventario = new List<Producto>()
        {
            new Producto("101", "Leche Entera 1L", 1.20, 50),
            new Producto("102", "Pan Molde", 1.80, 30),
            new Producto("103", "Arroz 1Kg", 0.95, 100),
            new Producto("104", "Café Instantáneo", 3.50, 20),
            new Producto("105", "Aceite de Girasol 1L", 2.40, 15)
        };

        private List<ItemCarrito> carrito = new List<ItemCarrito>();

        // Elementos visuales (Controles)
        private ListBox lstInventario;
        private ListBox lstCarrito;
        private TextBox txtCantidad;
        private Button btnAgregar;
        private Button btnPagar;
        private Button btnVaciar;
        private Label lblTotal;

        public Form1()
        {
            // Configuración de la Ventana Principal
            this.Text = "Minimercado Virtual - Punto de Venta";
            this.Size = new Size(750, 450);
            this.StartPosition = FormStartPosition.CenterScreen;

            InicializarComponentes();
            ActualizarInventarioVisual();
        }

        private void InicializarComponentes()
        {
            // Etiqueta Inventario
            Label lblInv = new Label() { Text = "Productos Disponibles:", Location = new Point(20, 15), Size = new Size(200, 20) };
            this.Controls.Add(lblInv);

            // Lista Visual de Inventario
            lstInventario = new ListBox() { Location = new Point(20, 40), Size = new Size(300, 250), Font = new Font("Courier New", 9) };
            this.Controls.Add(lstInventario);

            // Etiqueta Carrito
            Label lblCar = new Label() { Text = "Tu Carrito de Compras:", Location = new Point(400, 15), Size = new Size(200, 20) };
            this.Controls.Add(lblCar);

            // Lista Visual del Carrito
            lstCarrito = new ListBox() { Location = new Point(400, 40), Size = new Size(300, 250), Font = new Font("Courier New", 9) };
            this.Controls.Add(lstCarrito);

            // Selector de cantidad
            Label lblCant = new Label() { Text = "Cantidad:", Location = new Point(20, 310), Size = new Size(60, 20) };
            this.Controls.Add(lblCant);

            txtCantidad = new TextBox() { Location = new Point(85, 307), Size = new Size(50, 20), Text = "1" };
            this.Controls.Add(txtCantidad);

            // Botón Agregar al Carrito
            btnAgregar = new Button() { Text = "Agregar al Carrito »", Location = new Point(150, 305), Size = new Size(170, 25) };
            btnAgregar.Click += BtnAgregar_Click;
            this.Controls.Add(btnAgregar);

            // Etiqueta del Total a Pagar
            lblTotal = new Label() { Text = "TOTAL: $0.00", Location = new Point(400, 305), Size = new Size(300, 25), Font = new Font("Arial", 12, FontStyle.Bold), ForeColor = Color.DarkGreen };
            this.Controls.Add(lblTotal);

            // Botón Vaciar Carrito
            btnVaciar = new Button() { Text = "Vaciar Carrito", Location = new Point(400, 345), Size = new Size(130, 30) };
            btnVaciar.Click += BtnVaciar_Click;
            this.Controls.Add(btnVaciar);

            // Botón Pagar (Registrar Venta)
            btnPagar = new Button() { Text = "Registrar Venta (Pagar)", Location = new Point(540, 345), Size = new Size(160, 30), BackColor = Color.LightGreen };
            btnPagar.Click += BtnPagar_Click;
            this.Controls.Add(btnPagar);
        }

        // Lógica: Cargar productos en la pantalla
        private void ActualizarInventarioVisual()
        {
            lstInventario.Items.Clear();
            lstInventario.Items.Add(string.Format("{0,-6} {1,-18} {2,-8} {3,-5}", "ID", "Nombre", "Precio", "Stock"));
            lstInventario.Items.Add(new string('-', 45));

            foreach (var p in inventario)
            {
                lstInventario.Items.Add(string.Format("{0,-6} {1,-18} ${2,-7:F2} {3,-5}", p.ID, p.Nombre, p.Precio, p.Stock));
            }
        }

        // Lógica: Mostrar lo que hay en el carrito
        private void ActualizarCarritoVisual()
        {
            lstCarrito.Items.Clear();
            lstCarrito.Items.Add(string.Format("{0,-15} {1,-6} {2,-8}", "Producto", "Cant.", "Subtotal"));
            lstCarrito.Items.Add(new string('-', 35));

            foreach (var item in carrito)
            {
                lstCarrito.Items.Add(string.Format("{0,-15} {1,-6} ${2,-7:F2}", item.Producto.Nombre, item.Cantidad, item.Subtotal));
            }

            double subtotal = carrito.Sum(i => i.Subtotal);
            double totalConImpuesto = subtotal * 1.18; // 18% impuesto
            lblTotal.Text = $"TOTAL (+18% Imp.): ${totalConImpuesto:F2}";
        }

        // Evento al dar clic en "Agregar al Carrito"
        private void BtnAgregar_Click(object sender, EventArgs e)
        {
            if (lstInventario.SelectedIndex < 2) // Evita las cabeceras
            {
                MessageBox.Show("Por favor, seleccione un producto válido de la lista.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // Averiguar qué producto seleccionó el usuario basándonos en el índice
            int idx = lstInventario.SelectedIndex - 2;
            Producto prodSeleccionado = inventario[idx];

            if (!int.TryParse(txtCantidad.Text, out int cantidad) || cantidad <= 0)
            {
                MessageBox.Show("Ingrese una cantidad válida mayor a 0.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (cantidad > prodSeleccionado.Stock)
            {
                MessageBox.Show($"No hay suficiente stock. Solo quedan {prodSeleccionado.Stock} unidades.", "Sin Stock", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var itemExistente = carrito.FirstOrDefault(c => c.Producto.ID == prodSeleccionado.ID);
            if (itemExistente != null)
            {
                if (itemExistente.Cantidad + cantidad > prodSeleccionado.Stock)
                {
                    MessageBox.Show("La cantidad acumulada supera el stock disponible.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                itemExistente.Cantidad += cantidad;
            }
            else
            {
                carrito.Add(new ItemCarrito(prodSeleccionado, cantidad));
            }

            ActualizarCarritoVisual();
            txtCantidad.Text = "1"; // Resetear cuadro de texto
        }

        // Evento al dar clic en "Vaciar Carrito"
        private void BtnVaciar_Click(object sender, EventArgs e)
        {
            carrito.Clear();
            ActualizarCarritoVisual();
        }

        // Evento al dar clic en "Pagar"
        private void BtnPagar_Click(object sender, EventArgs e)
        {
            if (carrito.Count == 0)
            {
                MessageBox.Show("El carrito está vacío.", "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            double subtotal = carrito.Sum(i => i.Subtotal);
            double impuesto = subtotal * 0.18;
            double total = subtotal + impuesto;

            DialogResult resultado = MessageBox.Show($"¿Desea confirmar el pago por un total de ${total:F2}?", "Confirmar Compra", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (resultado == DialogResult.Yes)
            {
                // Descontar del inventario
                foreach (var item in carrito)
                {
                    item.Producto.Stock -= item.Cantidad;
                }

                // Generar un Ticket visual bonito usando un mensaje
                string ticket = $"=== TICKET DE COMPRA ===\nFecha: {DateTime.Now}\n\n";
                foreach (var item in carrito)
                {
                    ticket += $"{item.Producto.Nombre} x{item.Cantidad} = ${item.Subtotal:F2}\n";
                }
                ticket += $"\nSubtotal: ${subtotal:F2}\nImpuestos (18%): ${impuesto:F2}\nTOTAL PAGADO: ${total:F2}\n\n¡Gracias por su compra!";

                MessageBox.Show(ticket, "Venta Exitosa", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // Limpiar todo para la siguiente venta
                carrito.Clear();
                ActualizarCarritoVisual();
                ActualizarInventarioVisual();
            }
        }
    }
}
