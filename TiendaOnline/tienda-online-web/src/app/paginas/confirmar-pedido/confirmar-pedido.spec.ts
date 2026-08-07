import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ConfirmarPedido } from './confirmar-pedido';

describe('ConfirmarPedido', () => {
  let component: ConfirmarPedido;
  let fixture: ComponentFixture<ConfirmarPedido>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      imports: [ConfirmarPedido],
    }).compileComponents();

    fixture = TestBed.createComponent(ConfirmarPedido);
    component = fixture.componentInstance;
    await fixture.whenStable();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
